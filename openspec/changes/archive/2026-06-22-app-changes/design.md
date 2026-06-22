## Context

The app is a single-window WinForms tool. Both editors are plain `TextBox` controls configured with `WordWrap = false` and `ScrollBars = ScrollBars.Both`, which forces a horizontal scrollbar and keeps the vertical scrollbar permanently visible. The output is a `TextBox`, so it can only render a single foreground color — no per-token coloring is possible. The window opens maximized every time (`WindowState = Maximized`) with no persistence. Theme constants (fonts, colors) live in `Theme.cs`; format operations live in the UI-free `FormatService`.

The five requested improvements touch the shell (wrapping, scrollbars, window persistence, font size) and add a new concern (syntax highlighting of beautified output).

## Goals / Non-Goals

**Goals:**
- Wrap long lines in both editors and remove the horizontal scrollbar.
- Show the vertical scrollbar only when content overflows.
- Persist and restore window size/position/state under AppData.
- Increase editor font size by ~2 points.
- Color beautified JSON/XML output VS Code–style, including bracket-depth coloring.

**Non-Goals:**
- Live/as-you-type highlighting of the *input* editor (highlighting applies to Beautify output only).
- Highlighting Validate/Minify/Convert results or error messages.
- A full editor component (line numbers, folding, themes selector) or any external dependency.
- Changing `FormatService` behavior or the conversion semantics.

## Decisions

### Output editor becomes a `RichTextBox`

A plain `TextBox` cannot color text runs. Switch `txtOutput` to a `RichTextBox`, which supports `SelectionColor` per run and — crucially — also resolves two of the other requirements for free:
- `RichTextBox` with `WordWrap = true` (default) does not show a horizontal scrollbar.
- `RichTextBox.ScrollBars = RichTextBoxScrollBars.Vertical` shows the vertical scrollbar only when needed (unlike `TextBox`, whose scrollbar is always visible once enabled).

The input editor `txtInput` becomes a `RichTextBox` too (read/write, `ReadOnly = false`, `WordWrap = true`, `ScrollBars = RichTextBoxScrollBars.Vertical`). A multiline `TextBox` with `ScrollBars.Vertical` always paints the bar, so it cannot satisfy "only when needed"; `RichTextBox` auto-hides it. Making both editors `RichTextBox` keeps them consistent and gives auto-hiding scrollbars on both.

- Alternative — keep `TextBox` and toggle scrollbars manually by measuring text height: rejected as fragile (re-measuring on every resize/keystroke) versus `RichTextBox`'s built-in behavior.

### Highlighting via a UI-free tokenizer + a thin apply step

To match the project's separation of concerns (logic in UI-free, unit-testable classes like `FormatService`; UI in `Form1`), split the feature in two:

1. **`SyntaxHighlighter` (UI-free)** — a static class that takes the beautified text plus the `FormatKind` and returns an ordered list of token spans `(int Start, int Length, TokenColor)`. No WinForms dependency, so it is unit-testable on its own.
2. **`Form1` apply step** — iterates the returned spans and applies `Select`+`SelectionColor` to the `RichTextBox`, mapping each `TokenColor` to a `Theme` color.

Because the text is produced by our own beautifier (well-formed, predictable indentation), a lightweight tokenizer is sufficient:

- **JSON**: walk the string classifying tokens — property name (a string immediately followed by `:`), string value, number, `true`/`false`/`null`, and structural punctuation `{}[]:,`. The string scanner MUST consume escaped quotes (`\"`) so that `:`, `{`, `}` or quotes *inside* a string literal are never mistaken for structure. Track brace/bracket nesting depth and pick the delimiter color from a cycling palette by depth.
- **XML**: classify tag delimiters `< > />`, element names, attribute names, attribute values (quoted), and inner text.

**Line endings / offset alignment**: span offsets are only correct if the tokenizer analyzes the *exact* string that ends up in the `RichTextBox`. The current `Form1.Normalize` (CRLF conversion) exists for the multiline `TextBox`; `RichTextBox` renders `\n` fine and counts it as one character in its selection index space. The Beautify path therefore tokenizes and assigns `.Text` from the same single string (no CRLF re-expansion between tokenizing and applying) so offsets line up.

To keep flicker and cost down on large output, suspend redraw while writing (`WM_SETREDRAW` via P/Invoke), set the whole text once, apply the spans, then re-enable redraw and reset the selection to the start.

- Alternative — reuse `System.Text.Json`/`XmlReader` to tokenize: viable but maps awkwardly onto exact character offsets needed for `Select`. A small purpose-built tokenizer over our own normalized output is simpler and offset-accurate.

### Resetting formatting between actions

`RichTextBox` keeps per-run color formatting until explicitly cleared. After a colored Beautify, a subsequent Minify/Validate/Convert or an error message must not inherit leftover colors. The `Form1` render path therefore resets formatting before non-Beautify output: select all, set a single `SelectionColor` (success or error), then set the plain text. Coloring is applied only on the successful Beautify path.

### Token color palette in `Theme.cs`

Add VS Code Dark+–inspired colors: property/key, string, number, keyword (bool/null), XML tag, XML attribute name, plus a small ordered array of bracket-depth colors (e.g. gold / violet / light-blue cycling). Keep them in `Theme` so the palette stays centralized with the rest of the styling.

### Window-bounds persistence

Add a `WindowSettings` helper that serializes `{ X, Y, Width, Height, Maximized }` to a JSON file at `%AppData%\JsonXmlBeautify\window.json` (via `Environment.SpecialFolder.ApplicationData`). 
- On `Form.Load`: if a valid record exists, set `StartPosition = Manual`, apply bounds, and clamp against `Screen.AllScreens` working areas so an off-screen window is pulled back into view; restore `Maximized` if set. Otherwise fall back to the current maximized default.
- On `FormClosing`: if maximized, persist `RestoreBounds` (not the maximized bounds) plus the maximized flag; otherwise persist current bounds. Wrap file I/O in try/catch so a missing/corrupt file never blocks startup or shutdown.

Use `System.Text.Json` (already in use) for (de)serialization. Move the `WindowState = Maximized` decision out of the designer and into load logic so persistence governs it.

### Font size

Bump `Theme.EditorFont` from 11pt to 13pt (Consolas). UI font for buttons/toggle is unchanged.

## Risks / Trade-offs

- **Large beautified output makes per-run `SelectionColor` slow** → Repeated `Select`+`SelectionColor` is O(n²) in the worst case; suspending redraw removes flicker but not the algorithmic cost. Acceptable for a desktop utility's typical payloads, and highlighting only runs on explicit Beautify (not while typing). If it ever becomes a problem, the fallback is to build an RTF string directly. Recorded as a known limitation.
- **Tokenizer drift from real grammars** → It only ever runs on output our own beautifier produced (well-formed, predictable formatting), not arbitrary user input, so edge cases are bounded; malformed input never reaches the highlighter.
- **Corrupt or stale `window.json` (e.g. monitor unplugged)** → Validate against `Screen` working areas and clamp; treat any parse/IO failure as "no saved state" and fall back to maximized.
- **`RichTextBox` for input changes paste/behavior subtly** (e.g. RTF paste) → Force plain-text handling by reading `.Text`; conversions operate on `.Text` exactly as before, so `FormatService` is unaffected.

## Open Questions

- None outstanding. Bracket-depth palette is settled: VS Code Dark+ defaults (gold `#FFD700`, violet `#DA70D6`, light blue `#179FFF`) cycling every 3 levels. Editor stays `RichTextBox` (no external editor component).
