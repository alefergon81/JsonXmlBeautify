## ADDED Requirements

### Requirement: Application launches maximized

The application SHALL open in a maximized window when launched.

#### Scenario: Window opens maximized

- **WHEN** the user starts the application
- **THEN** the main window is displayed maximized, filling the available screen work area

### Requirement: Modern dark theme

The application SHALL present a modern, dark-themed visual style across the window and all controls (background, text fields, buttons, and toggle).

#### Scenario: Dark theme applied on startup

- **WHEN** the main window is displayed
- **THEN** the window background, both editors, the toggle, and the action buttons use dark-theme colors with readable contrast

### Requirement: Two-pane editor layout

The main window SHALL contain a left input editor and a right output editor, arranged side by side, with the action controls placed between them.

#### Scenario: Initial layout

- **WHEN** the main window is displayed
- **THEN** the left editor is empty and editable, the right editor is empty, and an action area sits between the two editors

#### Scenario: Editors resize with the window

- **WHEN** the window is resized or maximized
- **THEN** both editors expand or contract to fill the available space while the central action area remains between them

### Requirement: Left editor accepts input

The left editor SHALL allow the user to type or paste raw, unformatted text.

#### Scenario: User pastes input

- **WHEN** the user pastes text into the left editor
- **THEN** the text appears in the left editor and is available as the source for actions

### Requirement: Right editor displays results

The right editor SHALL start empty and display the result produced by the most recent action. The right editor SHALL be read-only for results, but its content MAY be selected and copied.

#### Scenario: Result populates right editor

- **WHEN** the user triggers an action that produces output
- **THEN** the right editor's content is replaced with the result of that action

### Requirement: Format toggle selects JSON or XML

The window SHALL provide a toggle that selects the active format, either JSON or XML, and all actions SHALL operate according to the currently selected format.

#### Scenario: Default format

- **WHEN** the application starts
- **THEN** the toggle has a defined default format (JSON) selected

#### Scenario: Switching format

- **WHEN** the user changes the toggle from JSON to XML or from XML to JSON
- **THEN** subsequent actions operate on the newly selected format and the Convert button label updates to reflect the target format

### Requirement: Action button bar

The central action area SHALL contain four buttons: Validate, Beautify, Minify, and Convert. Each button SHALL operate on the left editor's content and write to the right editor, respecting the selected format.

#### Scenario: Convert button label reflects direction

- **WHEN** the JSON format is selected
- **THEN** the convert button reads "Convert To XML"
- **WHEN** the XML format is selected
- **THEN** the convert button reads "Convert To JSON"

#### Scenario: Action operates on current input

- **WHEN** the user clicks any action button
- **THEN** the operation reads the current left-editor text and writes its output to the right editor
