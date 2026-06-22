using System.Drawing;

namespace JsonXmlBeautify
{
    /// <summary>
    /// Centralized dark-theme palette and shared fonts used across the UI.
    /// </summary>
    internal static class Theme
    {
        // Surfaces
        public static readonly Color Background = Color.FromArgb(30, 30, 30);     // #1E1E1E window
        public static readonly Color Surface = Color.FromArgb(37, 37, 38);        // #252526 editors
        public static readonly Color Border = Color.FromArgb(62, 62, 66);         // #3E3E42

        // Text
        public static readonly Color Text = Color.FromArgb(212, 212, 212);        // #D4D4D4
        public static readonly Color SubtleText = Color.FromArgb(140, 140, 140);
        public static readonly Color OnAccent = Color.White;

        // Accent (buttons / active toggle)
        public static readonly Color Accent = Color.FromArgb(14, 99, 156);        // #0E639C
        public static readonly Color AccentHover = Color.FromArgb(17, 119, 187);  // #1177BB

        // Status colors for output
        public static readonly Color Success = Color.FromArgb(78, 201, 176);
        public static readonly Color Error = Color.FromArgb(244, 135, 113);

        // Syntax-highlighting token colors (VS Code Dark+ inspired)
        public static readonly Color SyntaxProperty = Color.FromArgb(156, 220, 254); // #9CDCFE keys / XML attribute names
        public static readonly Color SyntaxString = Color.FromArgb(206, 145, 120);   // #CE9178 string values
        public static readonly Color SyntaxNumber = Color.FromArgb(181, 206, 168);   // #B5CEA8 numeric values
        public static readonly Color SyntaxKeyword = Color.FromArgb(86, 156, 214);   // #569CD6 true/false/null, XML tags
        public static readonly Color SyntaxPunctuation = Text;                       // default structural punctuation

        // Bracket/brace colors cycled by nesting depth (VS Code Dark+ defaults).
        public static readonly Color[] BracketColors =
        {
            Color.FromArgb(255, 215, 0),    // #FFD700 gold
            Color.FromArgb(218, 112, 214),  // #DA70D6 violet
            Color.FromArgb(23, 159, 255),   // #179FFF light blue
        };

        // Fonts (Consolas ships on every supported Windows; safe monospace choice)
        public static readonly Font EditorFont = new("Consolas", 13F, FontStyle.Regular, GraphicsUnit.Point);
        public static readonly Font UiFont = new("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
    }
}
