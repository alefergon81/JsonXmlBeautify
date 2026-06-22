## ADDED Requirements

### Requirement: Validate input

The system SHALL validate whether the left-editor content is well-formed according to the selected format and SHALL report the outcome in the right editor.

#### Scenario: Valid JSON

- **WHEN** JSON is selected and the left editor contains well-formed JSON
- **AND** the user clicks Validate
- **THEN** the right editor reports that the JSON is valid

#### Scenario: Invalid JSON

- **WHEN** JSON is selected and the left editor contains malformed JSON
- **AND** the user clicks Validate
- **THEN** the right editor reports that the JSON is invalid, including the error message and (where available) the line/position of the error

#### Scenario: Valid XML

- **WHEN** XML is selected and the left editor contains well-formed XML
- **AND** the user clicks Validate
- **THEN** the right editor reports that the XML is valid

#### Scenario: Invalid XML

- **WHEN** XML is selected and the left editor contains malformed XML
- **AND** the user clicks Validate
- **THEN** the right editor reports that the XML is invalid, including the error message and (where available) the line/position of the error

### Requirement: Beautify input

The system SHALL pretty-print well-formed input with indentation according to the selected format and write the result to the right editor.

#### Scenario: Beautify JSON

- **WHEN** JSON is selected and the left editor contains well-formed JSON
- **AND** the user clicks Beautify
- **THEN** the right editor contains the same JSON, indented across multiple lines

#### Scenario: Beautify XML

- **WHEN** XML is selected and the left editor contains well-formed XML
- **AND** the user clicks Beautify
- **THEN** the right editor contains the same XML, indented across multiple lines

#### Scenario: Beautify malformed input

- **WHEN** the left editor content is not well-formed for the selected format
- **AND** the user clicks Beautify
- **THEN** the right editor shows the parse error instead of formatted output

### Requirement: Minify input

The system SHALL compact well-formed input into a single line with insignificant whitespace removed, according to the selected format, and write the result to the right editor.

#### Scenario: Minify JSON

- **WHEN** JSON is selected and the left editor contains well-formed JSON
- **AND** the user clicks Minify
- **THEN** the right editor contains the same JSON with insignificant whitespace removed

#### Scenario: Minify XML

- **WHEN** XML is selected and the left editor contains well-formed XML
- **AND** the user clicks Minify
- **THEN** the right editor contains the same XML with insignificant whitespace removed

#### Scenario: Minify malformed input

- **WHEN** the left editor content is not well-formed for the selected format
- **AND** the user clicks Minify
- **THEN** the right editor shows the parse error instead of compacted output

### Requirement: Convert between JSON and XML

The system SHALL convert the left-editor content from the selected format to the other format and write the result to the right editor. The direction follows the toggle: JSON mode converts JSON→XML, XML mode converts XML→JSON. Conversion is best-effort and not guaranteed to round-trip: JSON whose top level is an array or has multiple members is wrapped under a single root element before producing XML, XML attributes and mixed text content map to conventional `@name`/`#text` JSON keys, and converted JSON values are strings since XML carries no type information.

#### Scenario: Convert JSON to XML

- **WHEN** JSON is selected and the left editor contains well-formed JSON
- **AND** the user clicks Convert
- **THEN** the right editor contains an equivalent XML representation of the input

#### Scenario: Convert XML to JSON

- **WHEN** XML is selected and the left editor contains well-formed XML
- **AND** the user clicks Convert
- **THEN** the right editor contains an equivalent JSON representation of the input

#### Scenario: Convert JSON without a single root

- **WHEN** JSON is selected and the left editor contains a top-level array or multiple members
- **AND** the user clicks Convert
- **THEN** the result is wrapped under a single synthetic root element so the output is well-formed XML

#### Scenario: Convert malformed input

- **WHEN** the left editor content is not well-formed for the selected format
- **AND** the user clicks Convert
- **THEN** the right editor shows the parse error instead of converted output

### Requirement: Empty input handling

The system SHALL handle empty input gracefully for every action without throwing an unhandled exception.

#### Scenario: Action on empty input

- **WHEN** the left editor is empty
- **AND** the user clicks any action button
- **THEN** the right editor shows a clear message indicating there is no input to process
