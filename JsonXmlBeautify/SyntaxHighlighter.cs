using System;
using System.Collections.Generic;

namespace JsonXmlBeautify
{
    /// <summary>Semantic color role assigned to a token span.</summary>
    internal enum TokenColor
    {
        Property,   // JSON keys, XML attribute names
        String,     // string / quoted values
        Number,     // numeric values
        Keyword,    // true/false/null, XML element names
        Bracket0,   // brace/bracket nesting depth, cycled every 3 levels
        Bracket1,
        Bracket2
    }

    /// <summary>A colored range over the beautified text: [Start, Start+Length).</summary>
    internal readonly record struct TokenSpan(int Start, int Length, TokenColor Color);

    /// <summary>
    /// UI-free tokenizer that turns beautified JSON/XML into colored spans.
    /// It runs only over output produced by <see cref="FormatService"/> (well-formed,
    /// predictably indented), so a lightweight scanner is sufficient. Returns spans
    /// whose offsets are relative to the exact input string; the caller is responsible
    /// for placing that same string into the editor so offsets line up.
    /// </summary>
    internal static class SyntaxHighlighter
    {
        public static List<TokenSpan> Highlight(string text, FormatKind format) =>
            format == FormatKind.Json ? HighlightJson(text) : HighlightXml(text);

        // ---- JSON -------------------------------------------------------

        private static List<TokenSpan> HighlightJson(string text)
        {
            var spans = new List<TokenSpan>();
            int depth = 0;
            int i = 0;
            int n = text.Length;

            while (i < n)
            {
                char c = text[i];

                if (c == '{' || c == '[')
                {
                    spans.Add(new TokenSpan(i, 1, BracketColor(depth)));
                    depth++;
                    i++;
                }
                else if (c == '}' || c == ']')
                {
                    depth = Math.Max(0, depth - 1);
                    spans.Add(new TokenSpan(i, 1, BracketColor(depth)));
                    i++;
                }
                else if (c == '"')
                {
                    int start = i;
                    i++; // past opening quote
                    while (i < n)
                    {
                        if (text[i] == '\\') { i += 2; continue; } // skip escaped char (incl. \")
                        if (text[i] == '"') { i++; break; }
                        i++;
                    }

                    // A string is a property name when the next non-space char is ':'.
                    int j = i;
                    while (j < n && char.IsWhiteSpace(text[j])) j++;
                    bool isKey = j < n && text[j] == ':';

                    spans.Add(new TokenSpan(start, i - start, isKey ? TokenColor.Property : TokenColor.String));
                }
                else if (c == '-' || char.IsDigit(c))
                {
                    int start = i;
                    i++;
                    while (i < n && (char.IsDigit(text[i]) || text[i] is '.' or 'e' or 'E' or '+' or '-'))
                        i++;
                    spans.Add(new TokenSpan(start, i - start, TokenColor.Number));
                }
                else if (TryMatchKeyword(text, i, out int kwLen))
                {
                    spans.Add(new TokenSpan(i, kwLen, TokenColor.Keyword));
                    i += kwLen;
                }
                else
                {
                    i++; // whitespace and punctuation (':', ',') keep the default color
                }
            }

            return spans;
        }

        private static bool TryMatchKeyword(string text, int i, out int length)
        {
            foreach (string kw in new[] { "true", "false", "null" })
            {
                if (i + kw.Length <= text.Length &&
                    string.CompareOrdinal(text, i, kw, 0, kw.Length) == 0)
                {
                    length = kw.Length;
                    return true;
                }
            }
            length = 0;
            return false;
        }

        // ---- XML --------------------------------------------------------

        private static List<TokenSpan> HighlightXml(string text)
        {
            var spans = new List<TokenSpan>();
            int i = 0;
            int n = text.Length;

            while (i < n)
            {
                if (text[i] != '<') { i++; continue; } // element text content: default color

                if (Matches(text, i, "<!--"))
                {
                    int end = text.IndexOf("-->", i, StringComparison.Ordinal);
                    i = end < 0 ? n : end + 3;
                    continue;
                }
                if (Matches(text, i, "<![CDATA["))
                {
                    int end = text.IndexOf("]]>", i, StringComparison.Ordinal);
                    i = end < 0 ? n : end + 3;
                    continue;
                }

                i++; // past '<'
                if (i < n && text[i] == '?') i++;  // processing instruction / declaration
                if (i < n && text[i] == '/') i++;  // closing tag

                int nameStart = i;
                while (i < n && IsNameChar(text[i])) i++;
                if (i > nameStart)
                    spans.Add(new TokenSpan(nameStart, i - nameStart, TokenColor.Keyword));

                // attributes until end of tag
                while (i < n && text[i] != '>')
                {
                    char ch = text[i];
                    if (ch == '"' || ch == '\'')
                    {
                        int qStart = i;
                        char quote = ch;
                        i++;
                        while (i < n && text[i] != quote) i++;
                        if (i < n) i++; // closing quote
                        spans.Add(new TokenSpan(qStart, i - qStart, TokenColor.String));
                    }
                    else if (IsNameChar(ch))
                    {
                        int aStart = i;
                        while (i < n && IsNameChar(text[i])) i++;
                        spans.Add(new TokenSpan(aStart, i - aStart, TokenColor.Property));
                    }
                    else
                    {
                        i++; // whitespace, '=', '/', '?' keep the default color
                    }
                }

                if (i < n && text[i] == '>') i++;
            }

            return spans;
        }

        private static bool IsNameChar(char c) =>
            char.IsLetterOrDigit(c) || c is '_' or '-' or ':' or '.';

        private static bool Matches(string text, int i, string token) =>
            i + token.Length <= text.Length &&
            string.CompareOrdinal(text, i, token, 0, token.Length) == 0;

        // ---- Helpers ----------------------------------------------------

        private static TokenColor BracketColor(int depth) =>
            (depth % 3) switch
            {
                0 => TokenColor.Bracket0,
                1 => TokenColor.Bracket1,
                _ => TokenColor.Bracket2
            };
    }
}
