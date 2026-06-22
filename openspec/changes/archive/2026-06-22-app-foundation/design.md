## Context

JsonXmlBeautify is a .NET 10 WinForms app (`net10.0-windows`, nullable + implicit usings) currently containing only the default scaffold. This change builds the first functional version: the window shell and the JSON/XML text operations. WinForms has no built-in dark theme, so the dark styling must be applied manually. Validate/Beautify/Minify rely on the .NET base class library (`System.Text.Json`, `System.Xml`); JSON↔XML conversion adds a single third-party package (`Newtonsoft.Json`), since the BCL has no first-class JSON↔XML bridge.

## Goals / Non-Goals

**Goals:**
- A maximized, modern dark-themed single window with a two-pane layout and a central action bar.
- A JSON/XML toggle that drives all four actions (Validate, Beautify, Minify, Convert).
- Format/operation logic separated from UI so it is unit-testable later.
- Robust error handling: malformed input and empty input never crash the app.

**Non-Goals:**
- No file open/save, drag-and-drop, syntax highlighting, or line numbers in this change.
- No settings persistence (theme/format choice is not remembered between runs).
- No test project yet (logic is structured to allow tests later, but tests are out of scope here).
- No round-trip fidelity guarantee for JSON↔XML edge cases beyond what `System.Xml`/`System.Text.Json` provide.

## Decisions

### Layout: SplitContainer with a center action panel
Use a root `TableLayoutPanel` (or a `SplitContainer` for the editors) with three regions: left editor, center action panel, right editor. The center panel is a fixed-width `FlowLayoutPanel`/`TableLayoutPanel` holding the toggle and four buttons stacked vertically. Editors use `Dock = Fill` so they resize with the window.
- *Alternative considered*: a single `SplitContainer` with buttons overlaid — rejected as harder to lay out reliably. A three-column `TableLayoutPanel` (auto-size center column) is simpler and resizes cleanly.

### Format toggle: custom sliding switch control
The JSON/XML toggle is a **custom sliding switch** (a small `Control` subclass, e.g. `ToggleSwitch`), not a stock `CheckBox` or `RadioButton`. It renders a rounded track with a knob that animates from the left side (JSON) to the right side (XML), with the active label highlighted — giving the "modern" flip-switch feel the user asked for. It exposes a boolean/enum state plus a `Toggled`/`CheckedChanged` event the form subscribes to (to drive operations and the Convert button label). Implemented by overriding `OnPaint` (anti-aliased rounded rectangle + knob) and `OnClick`, with an optional `Timer`-driven knob animation.
- *Alternatives considered*: a stock `CheckBox` (functional but not the requested switch look) or two `RadioButton`s (no sliding motion) — both rejected because the user specifically wants a key/switch that slides side to side. Custom paint keeps zero extra dependencies.

### Editors: multiline TextBox vs RichTextBox
Use `TextBox` (Multiline, `ScrollBars.Both`, `WordWrap = false`, monospace font e.g. Consolas/Cascadia Mono). The right editor is `ReadOnly = true` but still selectable/copyable.
- *Alternative considered*: `RichTextBox` — unnecessary since we don't do syntax coloring in this change; `TextBox` is lighter.

### Dark theme: manual control styling
Apply a centralized palette in code (e.g. a small `Theme` static class) setting `BackColor`/`ForeColor` on the form and controls. Set the form's `BackColor` to a dark gray (~#1E1E1E), editors slightly lighter (~#252526), text near-white (~#D4D4D4), and accent for buttons (~#0E639C / hover lighter). Use `FlatStyle = Flat` with `FlatAppearance` for buttons to remove the default 3D chrome. Optionally set the title bar dark via the `DwmSetWindowAttribute` `DWMWA_USE_IMMERSIVE_DARK_MODE` P/Invoke.
- *Alternative considered*: a third-party theming library (e.g. a NuGet dark-theme pack) — rejected to keep zero external dependencies for this foundational change.

### Format operations: a stateless service class
Create a `FormatService` (or two: `JsonService`, `XmlService`) with pure methods: `Validate`, `Beautify`, `Minify`, `Convert`, each taking input text and returning a result object (success + text, or failure + error message). The form's button handlers call these based on the toggle. This keeps logic out of the UI and testable.
- JSON: `System.Text.Json` — `JsonDocument.Parse` for validate; `JsonSerializer`/`Utf8JsonWriter` with `WriteIndented` for beautify; writer without indentation for minify.
- XML: `System.Xml.Linq.XDocument`/`XmlDocument` — `Parse` for validate; `XDocument.ToString()` (indented) for beautify; `SaveOptions.DisableFormatting` for minify.
- Convert: use `Newtonsoft.Json`'s `JsonConvert.DeserializeXmlNode` (JSON→XML) and `JsonConvert.SerializeXmlNode` (XML→JSON), the de-facto standard for this conversion (decided — see below). Conversion is **best-effort**, not a guaranteed round-trip:
  - JSON→XML requires a single root; a JSON value that is an array or has multiple top-level members is wrapped under a synthetic root element (e.g. `root`) before conversion.
  - XML→JSON maps attributes to `@name` keys and mixed text content to `#text`, per Newtonsoft's convention.
  - XML carries no type information, so all converted values are strings on the JSON side.

### Error reporting
Each operation returns a structured result; on failure the right editor displays a clear, human-readable error (message + line/position when the parser exposes it). Catch `JsonException` (has `LineNumber`/`BytePositionInLine`) and `XmlException` (has `LineNumber`/`LinePosition`).

## Risks / Trade-offs

- **WinForms dark theme is manual and imperfect** (scrollbars, some native chrome stay light) → Mitigate by styling all our own controls and applying the immersive-dark-mode title bar; accept that native scrollbar coloring may remain default.
- **JSON↔XML conversion has inherent ambiguities** (attributes vs elements, arrays, root element naming, type loss) → Mitigate by using a well-known conversion approach (Newtonsoft `JsonConvert` XML helpers) and documenting that conversion is best-effort, not guaranteed round-trip.
- **`Newtonsoft.Json` is an added third-party dependency** → Accepted: the BCL has no first-class JSON↔XML bridge, and Newtonsoft is the well-established standard; the cost is one well-maintained package.
- **Custom `ToggleSwitch` is hand-painted** (no stock control) → Mitigate by keeping it a small, self-contained `Control` with simple `OnPaint`/`OnClick` logic; animation is optional polish and can degrade to an instant flip.
- **Large inputs** could make the UI sluggish since operations run on the UI thread → Acceptable for a foundation; revisit with async/background work if needed.

## Migration Plan

Not applicable — additive change to a scaffold-only app with no existing users or data. Rollback = revert the commit.

## Resolved Decisions

- **JSON↔XML conversion library**: **Decided — adopt `Newtonsoft.Json`** (`JsonConvert.DeserializeXmlNode`/`SerializeXmlNode`). It is the pragmatic standard for this conversion and the only third-party package this change introduces.
- **Format toggle control**: **Decided — custom sliding switch** (`ToggleSwitch`) that animates a knob from JSON (left) to XML (right), per the "Format toggle" decision above.
- **Immersive dark title bar**: **Decided — include it** via the `DwmSetWindowAttribute` (`DWMWA_USE_IMMERSIVE_DARK_MODE`) P/Invoke, for a cohesive dark look.
