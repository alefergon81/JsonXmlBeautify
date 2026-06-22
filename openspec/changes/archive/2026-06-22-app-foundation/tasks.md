## 1. Project setup

- [x] 1.1 Add the `Newtonsoft.Json` package reference to the project and restore packages
- [x] 1.2 Add a `Theme` static class with the dark palette (background, editor, text, accent/hover colors) and a shared monospace font definition

## 2. Window shell (app-shell)

- [x] 2.1 Set the form to start maximized (`WindowState = Maximized`) and give it a sensible title
- [x] 2.2 Build the layout in the designer: root `TableLayoutPanel` with left editor column, fixed-width center action column, and right editor column; editors `Dock = Fill`
- [x] 2.3 Add the left input editor (multiline `TextBox`, scrollbars, no word wrap, monospace font, editable)
- [x] 2.4 Add the right output editor (multiline `TextBox`, `ReadOnly`, scrollbars, monospace, still selectable)
- [x] 2.5 Build a custom `ToggleSwitch` control (rounded track + knob, sliding/animated from JSON-left to XML-right via `OnPaint`/`OnClick`, exposing state + a change event) and add it to the center panel, defaulting to JSON
- [x] 2.6 Add the four action buttons (Validate, Beautify, Minify, Convert) to the center panel with flat styling
- [x] 2.7 Apply the dark theme to the form and all controls (call from `Form1` constructor / load)
- [x] 2.8 Apply the immersive dark title bar via `DwmSetWindowAttribute` P/Invoke
- [x] 2.9 Wire the toggle so changing format updates the Convert button label ("Convert To XML" in JSON mode, "Convert To JSON" in XML mode)

## 3. Format operations (text-operations)

- [x] 3.1 Create a result type carrying success/failure, output text, and an error message (with line/position when available)
- [x] 3.2 Implement JSON operations: validate, beautify (indented), minify (compact) using `System.Text.Json`, mapping `JsonException` line/position to the error message
- [x] 3.3 Implement XML operations: validate, beautify (indented), minify (compact) using `System.Xml.Linq`/`XmlDocument`, mapping `XmlException` line/position to the error message
- [x] 3.4 Implement Convert: JSON→XML when in JSON mode, XML→JSON when in XML mode
- [x] 3.5 Handle empty input in every operation by returning a clear "no input" message instead of throwing

## 4. Wire UI to operations

- [x] 4.1 Implement Validate click: run the selected format's validate, write success or error to the right editor
- [x] 4.2 Implement Beautify click: run beautify, write output or parse error to the right editor
- [x] 4.3 Implement Minify click: run minify, write output or parse error to the right editor
- [x] 4.4 Implement Convert click: run convert in the toggle's direction, write output or parse error to the right editor

## 5. Verify

- [x] 5.1 Build the solution (`dotnet build JsonXmlBeautify.slnx`) and resolve any errors
- [x] 5.2 Run the app (`dotnet run --project JsonXmlBeautify`); confirm it opens maximized with the dark theme and two-pane layout
- [x] 5.3 Manually exercise each button in both JSON and XML modes with valid, invalid, and empty input; confirm outputs and error messages match the specs
