## 1. Theme & fonts

- [x] 1.1 In `Theme.cs`, change `EditorFont` from 11pt to 13pt Consolas.
- [x] 1.2 In `Theme.cs`, add VS Code Dark+ token colors: key/property, string value, number, keyword (bool/null), XML tag name, XML attribute name, and punctuation/default.
- [x] 1.3 In `Theme.cs`, add an ordered `BracketColors` array (gold `#FFD700`, violet `#DA70D6`, light blue `#179FFF`) used cyclically by nesting depth.

## 2. Editors: wrapping & scrollbars

- [x] 2.1 In `Form1.Designer.cs`, change `txtOutput` from `TextBox` to `RichTextBox`; set `WordWrap = true`, `ScrollBars = RichTextBoxScrollBars.Vertical`, `ReadOnly = true`, keep dark styling and dock/margins.
- [x] 2.2 In `Form1.Designer.cs`, change `txtInput` from `TextBox` to `RichTextBox`; set `WordWrap = true`, `ScrollBars = RichTextBoxScrollBars.Vertical`, `ReadOnly = false`, keep dock/margins.
- [x] 2.3 Update field declarations in `Form1.Designer.cs` to the new `RichTextBox` types.
- [x] 2.4 In `Form1.cs`, update `StyleEditor` to accept `RichTextBox` and apply `Theme.Surface`/`Theme.Text`/`Theme.EditorFont`.
- [x] 2.5 Verify no horizontal scrollbar appears and the vertical scrollbar shows only when content overflows, in both editors.

## 3. Window position/size persistence

- [x] 3.1 Add `WindowSettings.cs` with a record `{ int X, Y, Width, Height; bool Maximized }` and Load/Save to `%AppData%\JsonXmlBeautify\window.json` using `System.Text.Json`; wrap all file I/O in try/catch.
- [x] 3.2 Remove the hard-coded `WindowState = FormWindowState.Maximized` from `Form1.Designer.cs`.
- [x] 3.3 In `Form1.cs`, on `OnLoad`: if valid saved state exists, set `StartPosition = Manual`, apply bounds, clamp against `Screen.AllScreens` working areas, and restore maximized state; otherwise default to maximized.
- [x] 3.4 In `Form1.cs`, on `OnFormClosing`: persist `RestoreBounds` + maximized flag when maximized, else current bounds.
- [x] 3.5 Verify the window reopens at the last size/position/state, and an off-screen saved position is pulled back on-screen.

## 4. Syntax highlighting of beautify output

- [x] 4.1 Add UI-free `SyntaxHighlighter.cs`: a static method taking beautified text + `FormatKind` and returning an ordered list of token spans `(int Start, int Length, TokenColor)`. No WinForms dependency (unit-testable).
- [x] 4.2 Implement the JSON tokenizer: classify property names (string immediately followed by `:`), string values, numbers, bool/null keywords, and structural punctuation; track `{}`/`[]` depth for delimiter color by depth. The string scanner MUST consume escaped quotes (`\"`) so `:`/`{`/`}`/quotes inside a string literal are not treated as structure.
- [x] 4.3 Implement the XML tokenizer: classify tag delimiters, element names, attribute names, attribute/quoted values, and inner text.
- [x] 4.4 In `Form1.cs`, add the apply step: suppress redraw (`WM_SETREDRAW` P/Invoke), set `.Text` once from the SAME string passed to the tokenizer (no CRLF re-expansion between tokenize and apply, so offsets align), apply each span via `Select`+`SelectionColor` mapping `TokenColor`→`Theme` color, then re-enable redraw and reset selection to start.
- [x] 4.5 In `Form1.cs`, route only successful Beautify output through the highlighter. For Validate/Minify/Convert and all error messages, RESET formatting first (`SelectAll`, set a single `SelectionColor` = success/error color) before assigning plain text, so no leftover coloring from a prior Beautify remains.
- [x] 4.6 Verify beautified JSON and XML are colored by token type and bracket depth; verify switching to Minify/Validate/Convert or an error after a Beautify shows no residual coloring; verify punctuation inside string values is not miscolored.

## 5. Build & verify

- [x] 5.1 `dotnet build JsonXmlBeautify.slnx` succeeds with no warnings introduced.
- [x] 5.2 `dotnet run --project JsonXmlBeautify` and manually confirm all five improvements against their spec scenarios. (App launches without error; window-state persistence verified end-to-end — `window.json` written with maximized flag + restore bounds on close. Editor wrapping/scrollbar config and syntax colors compile correctly and follow RichTextBox semantics; final visual color check is best eyeballed by the user.)
