## Why

JsonXmlBeautify is currently just the default WinForms scaffold (`Form1` is empty) with no functionality. To deliver the intended product — a single-window utility for working with JSON and XML — we need to build the application foundation: the window shell, its modern dark-themed layout, and the core text operations (validate, beautify, minify, convert).

## What Changes

- Build the main window UI with a modern, dark visual style.
- Launch the application **maximized** on startup.
- Add a **two-pane layout**: a left editor for raw/unformatted input and a right editor (starts empty) that displays the result of each action.
- Add a **JSON ⇄ XML toggle** that selects which format the actions operate on.
- Add four **action buttons** between the two editors that operate on the left input and write to the right output, respecting the toggle:
  - **Validate** — report whether the input is well-formed (success or the error details).
  - **Beautify** — pretty-print with indentation.
  - **Minify** — strip whitespace into a single compact line.
  - **Convert To X** — convert JSON→XML when in JSON mode, XML→JSON when in XML mode; the button label reflects the target format.

## Capabilities

### New Capabilities
- `app-shell`: The main window — modern dark theme, maximized startup, two-pane editor layout, the JSON/XML format toggle, and the action button bar.
- `text-operations`: The validate, beautify, minify, and convert operations for JSON and XML, including input parsing, error reporting, and conversion between the two formats.

### Modified Capabilities
<!-- None — this is the first functional change; no existing specs. -->

## Impact

- **Code**: `JsonXmlBeautify/Form1.cs` and `Form1.Designer.cs` (UI + event handlers); possibly new helper/service classes for the format operations. `Program.cs` unchanged (already runs `Form1`).
- **Dependencies**: JSON handled via `System.Text.Json` (in the .NET runtime); XML via `System.Xml`/`System.Xml.Linq`. JSON↔XML conversion uses **`Newtonsoft.Json`** (`JsonConvert.DeserializeXmlNode`/`SerializeXmlNode`) — the one third-party package added by this change.
- **Platform**: Windows-only (`net10.0-windows`), unchanged.
- **Tests**: No test project exists yet; conversion/format logic should be structured so it can be unit-tested later.
