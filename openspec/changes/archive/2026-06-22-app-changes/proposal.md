## Why

The current editors force a horizontal scrollbar and always show a vertical scrollbar, the window never remembers where the user left it, the editor text is small, and beautified output is a flat single color. These rough edges make the tool harder to read and less pleasant to use day to day.

## What Changes

- Both editors wrap long lines automatically and no longer show a horizontal scrollbar.
- The vertical scrollbar appears only when content overflows the editor height, instead of always being visible.
- The window remembers its size and position between runs, persisted under the user's AppData folder, and reopens in the same place.
- The editor font size increases by ~2 points for better readability.
- Beautified output is syntax-colored in a VS Code–like style (distinct colors for braces/brackets by nesting depth, property names, string values, numeric values, booleans/null, and XML tags/attributes) to aid visual scanning.

## Capabilities

### New Capabilities
- `output-syntax-highlighting`: Color-codes the beautified result in the output editor by token type (and bracket nesting depth) so structure is easy to scan, in a VS Code–like dark palette.

### Modified Capabilities
- `app-shell`: Editors wrap lines and suppress the horizontal scrollbar; the vertical scrollbar is shown only when needed; the window persists and restores its size/position; editor font size increases.

## Impact

- `JsonXmlBeautify/Form1.Designer.cs` — editor controls switch to wrapping with auto vertical scrollbar; output editor becomes a `RichTextBox` to support colored runs.
- `JsonXmlBeautify/Form1.cs` — load/save window bounds, apply highlighting on Beautify output.
- `JsonXmlBeautify/Theme.cs` — larger editor font; new token color palette.
- New file(s) for window-bounds persistence and a syntax highlighter/tokenizer.
- No external dependencies added; uses BCL `RichTextBox` and existing JSON/XML parsers.
