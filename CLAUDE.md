# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

JsonXmlBeautify is a .NET 10 Windows Forms desktop application (`net10.0-windows`, C# with nullable reference types and implicit usings enabled). It is a single-window utility for working with JSON and XML text, with a dark VS Code–like theme.

The implemented product:

- **Format toggle** — a custom sliding `ToggleSwitch` selecting whether operations target JSON (default) or XML.
- **Two text fields** — both are `RichTextBox` editors. The left field holds the raw/unformatted input typed or pasted by the user; the right field starts empty and shows the result of the chosen action (read-only, but selectable/copyable). Both wrap long lines (no horizontal scrollbar) and show a vertical scrollbar only when content overflows.
- **Action buttons (between the two fields)** — operate on the left input and write to the right output, respecting the JSON/XML toggle:
  - **Validate** — check whether the input is well-formed; report success or the parse error (with line/position where available).
  - **Beautify** — pretty-print with indentation; successful output is syntax-colored (VS Code Dark+ style, including bracket-by-nesting-depth coloring).
  - **Minify** — strip insignificant whitespace into a single compact line.
  - **Convert** — convert between JSON and XML (direction follows the toggle; label reads "Convert To XML"/"Convert To JSON").
- **Window placement** — the window remembers its size, position, and maximized state between runs (persisted under `%AppData%`), falling back to maximized on first launch.

## Commands

Run from the repository root.

```sh
dotnet build JsonXmlBeautify.slnx              # build the solution
dotnet run --project JsonXmlBeautify           # build and launch the app
dotnet build JsonXmlBeautify.slnx -c Release   # release build
```

There is no test project yet. To add one: `dotnet new xunit -o <Name>.Tests`, then add it to `JsonXmlBeautify.slnx`. Run tests with `dotnet test`.

## Structure

- `JsonXmlBeautify.slnx` — solution file (new XML-based `.slnx` format) referencing the single project.
- `JsonXmlBeautify/Program.cs` — STA entry point; calls `ApplicationConfiguration.Initialize()` then runs `Form1`.
- `JsonXmlBeautify/Form1.cs` + `Form1.Designer.cs` — the main window. UI controls and layout live in the `.Designer.cs` `InitializeComponent()` (3-column `TableLayoutPanel`: input | center action column | output); theme application, event wiring, and result rendering live in `Form1.cs`. `Form1.cs` also enables the immersive dark title bar via `DwmSetWindowAttribute`, restores/saves window placement in `OnLoad`/`OnFormClosing`, and applies syntax-coloring runs to the output `RichTextBox` (toggling redraw via `WM_SETREDRAW` while coloring). Output text is normalized to `\n` so highlighter offsets stay aligned with `RichTextBox` selection indices.
- `JsonXmlBeautify/FormatService.cs` — UI-free static class holding all logic: `Validate`/`Beautify`/`Minify`/`Convert`, keyed by the `FormatKind` enum (Json/Xml). JSON/XML formatting uses the BCL (`System.Text.Json`, `System.Xml.Linq`); JSON↔XML conversion uses **Newtonsoft.Json** (`DeserializeXmlNode`/`SerializeXmlNode`). Kept dependency-free of the UI so it can be unit-tested later.
- `JsonXmlBeautify/FormatResult.cs` — immutable success/output-or-error result returned by `FormatService`.
- `JsonXmlBeautify/SyntaxHighlighter.cs` — UI-free tokenizer that turns beautified JSON/XML into an ordered list of `TokenSpan` `(Start, Length, TokenColor)` records; `Form1` maps `TokenColor` to `Theme` colors and applies them. Runs only over `FormatService` output (well-formed, predictably indented), so a lightweight scanner suffices.
- `JsonXmlBeautify/WindowSettings.cs` — serializes window placement `{ X, Y, Width, Height, Maximized }` to `%AppData%\JsonXmlBeautify\window.json` via `System.Text.Json`; all file I/O is best-effort (try/catch), so a missing/corrupt file just yields no saved state.
- `JsonXmlBeautify/Theme.cs` — centralized dark palette, shared fonts (13pt Consolas editor font), syntax-token colors, and a `BracketColors` array cycled by nesting depth.
- `JsonXmlBeautify/ToggleSwitch.cs` — custom-drawn JSON/XML sliding toggle control (`IsXml` property, `Toggled` event).

## Planning

- `openspec/` holds OpenSpec planning artifacts. Capability specs live in `openspec/specs/<capability>/spec.md` (currently `app-shell`, `text-operations`, and `output-syntax-highlighting`); in-flight changes live in `openspec/changes/<name>/`, and completed ones move to `openspec/changes/archive/<date>-<name>/`. Use the `/opsx:*` skills to propose, apply, and archive changes.

## Notes

- Windows-only target — building and running require Windows. The Bash tool here runs Git Bash, but `dotnet` commands work from either shell.
- Uses the **Newtonsoft.Json** NuGet package (for JSON↔XML conversion) alongside the BCL JSON/XML APIs.
- This is not a git repository.
