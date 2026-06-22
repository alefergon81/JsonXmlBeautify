## MODIFIED Requirements

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

## ADDED Requirements

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
