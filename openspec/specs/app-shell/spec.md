# app-shell Specification

## Purpose

Defines the main application window and its visual shell: how the window launches, its theme, the two-pane editor layout, the format toggle, and the action button bar that ties user interaction together.

## Requirements

### Requirement: Application launches maximized

The application SHALL remember its window size and position between runs, persisting them under the user's AppData folder, and SHALL restore that size and position on the next launch. On first launch, or when no valid saved state exists, the application SHALL open in a maximized window. The application SHALL ensure the restored window is visible on the currently available screens.

#### Scenario: First launch with no saved state

- **WHEN** the user starts the application and no saved window state exists
- **THEN** the main window is displayed maximized, filling the available screen work area

#### Scenario: Restore previous size and position

- **WHEN** the user starts the application and a valid saved window state exists
- **THEN** the main window opens at the same size, position, and maximized/normal state it had when last closed

#### Scenario: Window state is saved on close

- **WHEN** the user closes the application
- **THEN** the current window size, position, and maximized/normal state are written to a file under the user's AppData folder

#### Scenario: Off-screen saved state is corrected

- **WHEN** a saved position would place the window outside all currently available screens
- **THEN** the application repositions the window so it is visible on an available screen

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

### Requirement: Editors wrap long lines

Both editors SHALL wrap long lines to the editor width and SHALL NOT display a horizontal scrollbar.

#### Scenario: Long line wraps instead of scrolling horizontally

- **WHEN** an editor contains a line wider than the editor's visible width
- **THEN** the line wraps onto the next visual row and no horizontal scrollbar is shown

### Requirement: Vertical scrollbar shown only when needed

Each editor SHALL display a vertical scrollbar only when its content exceeds the visible height, and SHALL hide it otherwise.

#### Scenario: Short content shows no vertical scrollbar

- **WHEN** an editor's content fits within its visible height
- **THEN** no vertical scrollbar is displayed

#### Scenario: Overflowing content shows a vertical scrollbar

- **WHEN** an editor's content exceeds its visible height
- **THEN** a vertical scrollbar is displayed

### Requirement: Readable editor font size

The editors SHALL use a monospace font sized for comfortable reading, approximately two points larger than the prior 11-point editor font.

#### Scenario: Editor text uses the larger font

- **WHEN** the main window is displayed
- **THEN** both editors render their text in a monospace font of approximately 13 points
