# output-syntax-highlighting Specification

## Purpose

Defines how beautified output is syntax-colored in the right editor: which token types are colored, the palette used, and the conditions under which coloring is or is not applied.

## Requirements

### Requirement: Syntax-colored beautify output

When the Beautify action produces well-formed output, the system SHALL display that output in the right editor with color applied per token type, in a VS Code–like dark palette, to make structure easy to scan. Coloring SHALL apply to both JSON and XML according to the selected format.

#### Scenario: Beautified JSON is color-coded by token type

- **WHEN** JSON is selected and the user clicks Beautify on well-formed JSON
- **THEN** the right editor shows the indented JSON with distinct colors for property names, string values, numeric values, boolean/null literals, and structural punctuation

#### Scenario: Beautified XML is color-coded by token type

- **WHEN** XML is selected and the user clicks Beautify on well-formed XML
- **THEN** the right editor shows the indented XML with distinct colors for element tag names, attribute names, attribute/text values, and structural punctuation

#### Scenario: Bracket nesting depth is distinguishable by color

- **WHEN** beautified JSON contains nested objects or arrays
- **THEN** braces/brackets are colored by their nesting depth so that an opening delimiter and its matching closing delimiter share a color distinct from delimiters at adjacent depths

### Requirement: Highlighting limited to successful beautify results

The system SHALL apply syntax coloring only to successful Beautify output. Error messages and the results of Validate, Minify, and Convert SHALL be displayed without token coloring, using the existing success/error text colors.

#### Scenario: Parse error is not syntax-colored

- **WHEN** the user clicks Beautify on malformed input for the selected format
- **THEN** the right editor shows the parse error using the error text color and no token coloring is applied

#### Scenario: Non-beautify actions are not syntax-colored

- **WHEN** the user clicks Validate, Minify, or Convert
- **THEN** the right editor shows the result using the standard result text color without per-token coloring
