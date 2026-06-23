using System;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Xml;
using System.Xml.Linq;

namespace JsonXmlBeautify
{
    /// <summary>The format the operations target.</summary>
    internal enum FormatKind
    {
        Json,
        Xml
    }

    /// <summary>
    /// Stateless JSON/XML operations: validate, beautify, minify and convert.
    /// Kept free of any UI dependency so it can be unit-tested later.
    /// JSON/XML formatting uses the BCL; JSON↔XML conversion uses Newtonsoft.Json.
    /// </summary>
    internal static class FormatService
    {
        private const string EmptyMessage = "There is no input to process.";

        public static FormatResult Validate(string input, FormatKind format)
        {
            if (string.IsNullOrWhiteSpace(input))
                return FormatResult.Fail(EmptyMessage);

            return format == FormatKind.Json ? ValidateJson(input) : ValidateXml(input);
        }

        public static FormatResult Beautify(string input, FormatKind format)
        {
            if (string.IsNullOrWhiteSpace(input))
                return FormatResult.Fail(EmptyMessage);

            return format == FormatKind.Json
                ? WriteJson(input, indented: true)
                : WriteXml(input, indent: true);
        }

        public static FormatResult Minify(string input, FormatKind format)
        {
            if (string.IsNullOrWhiteSpace(input))
                return FormatResult.Fail(EmptyMessage);

            return format == FormatKind.Json
                ? WriteJson(input, indented: false)
                : WriteXml(input, indent: false);
        }

        public static FormatResult Convert(string input, FormatKind format)
        {
            if (string.IsNullOrWhiteSpace(input))
                return FormatResult.Fail(EmptyMessage);

            return format == FormatKind.Json ? ConvertJsonToXml(input) : ConvertXmlToJson(input);
        }

        // ---- JSON -------------------------------------------------------

        private static FormatResult ValidateJson(string input)
        {
            input = NormalizeJson(input);
            try
            {
                using var _ = JsonDocument.Parse(input);
                return FormatResult.Ok("✓ Valid JSON.");
            }
            catch (JsonException ex)
            {
                return FormatResult.Fail(JsonError(ex));
            }
        }

        private static FormatResult WriteJson(string input, bool indented)
        {
            input = NormalizeJson(input);
            try
            {
                using var doc = JsonDocument.Parse(input);
                var options = new JsonSerializerOptions
                {
                    WriteIndented = indented,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };
                return FormatResult.Ok(JsonSerializer.Serialize(doc.RootElement, options));
            }
            catch (JsonException ex)
            {
                return FormatResult.Fail(JsonError(ex));
            }
        }

        private static string JsonError(JsonException ex) =>
            $"✗ Invalid JSON.\r\n\r\n{ex.Message}";

        /// <summary>
        /// Logs frequently carry JSON that was embedded inside another string, so
        /// every quote arrives escaped (e.g. {\"key\":\"value\"}) and a normal parse
        /// fails. This unescapes one level by reading the input as a JSON string
        /// literal, turning \" back into ". The original input is returned unchanged
        /// when it is already valid JSON or is not recognizably escaped.
        /// </summary>
        private static string NormalizeJson(string input)
        {
            // Fast path: already well-formed JSON, nothing to do.
            try { using var _ = JsonDocument.Parse(input); return input; }
            catch (JsonException) { }

            string trimmed = input.Trim();

            // Only escaped quotes are worth unescaping; bail otherwise.
            if (!trimmed.Contains("\\\"")) return input;

            // Drop the surrounding quotes when the whole thing was copied as a
            // quoted string ("{\"key\":\"value\"}").
            if (trimmed.Length >= 2 && trimmed[0] == '"' && trimmed[^1] == '"')
                trimmed = trimmed[1..^1];

            // Wrap as a JSON string literal so the parser does the unescaping.
            // Raw control characters aren't legal inside a literal, so escape the
            // common ones first; their escaped forms unescape back afterwards.
            string literal = "\"" + trimmed
                .Replace("\r", "\\r")
                .Replace("\n", "\\n")
                .Replace("\t", "\\t") + "\"";

            try
            {
                using var doc = JsonDocument.Parse(literal);
                if (doc.RootElement.ValueKind == JsonValueKind.String)
                {
                    string unescaped = doc.RootElement.GetString() ?? input;
                    // Accept only if the unescaped text is itself valid JSON.
                    using var _ = JsonDocument.Parse(unescaped);
                    return unescaped;
                }
            }
            catch (JsonException) { }

            return input;
        }

        // ---- XML --------------------------------------------------------

        private static FormatResult ValidateXml(string input)
        {
            try
            {
                XDocument.Parse(input);
                return FormatResult.Ok("✓ Valid XML.");
            }
            catch (XmlException ex)
            {
                return FormatResult.Fail(XmlError(ex));
            }
        }

        private static FormatResult WriteXml(string input, bool indent)
        {
            try
            {
                var doc = XDocument.Parse(input);
                var settings = new XmlWriterSettings
                {
                    Indent = indent,
                    IndentChars = "  ",
                    OmitXmlDeclaration = doc.Declaration is null,
                    NewLineHandling = NewLineHandling.Replace
                };

                var sb = new StringBuilder();
                using (var writer = XmlWriter.Create(sb, settings))
                {
                    doc.Save(writer);
                }
                return FormatResult.Ok(sb.ToString());
            }
            catch (XmlException ex)
            {
                return FormatResult.Fail(XmlError(ex));
            }
        }

        private static string XmlError(XmlException ex) =>
            $"✗ Invalid XML.\r\n\r\n{ex.Message} (line {ex.LineNumber}, position {ex.LinePosition})";

        // ---- Convert ----------------------------------------------------

        private static FormatResult ConvertJsonToXml(string input)
        {
            input = NormalizeJson(input);
            try
            {
                var token = Newtonsoft.Json.Linq.JToken.Parse(input);

                // XML needs a single root. An object converts directly under a
                // "root" element; an array or primitive is first wrapped as
                // {"item": <value>} so the result stays well-formed.
                string json = token is Newtonsoft.Json.Linq.JObject
                    ? input
                    : new Newtonsoft.Json.Linq.JObject { ["item"] = token }.ToString();

                XmlDocument? xml = Newtonsoft.Json.JsonConvert.DeserializeXmlNode(json, "root");
                if (xml is null)
                    return FormatResult.Fail("✗ Could not convert the JSON to XML.");

                return FormatResult.Ok(PrettyPrintXml(xml));
            }
            catch (Newtonsoft.Json.JsonException ex)
            {
                return FormatResult.Fail($"✗ Invalid JSON.\r\n\r\n{ex.Message}");
            }
        }

        private static FormatResult ConvertXmlToJson(string input)
        {
            try
            {
                var xml = new XmlDocument();
                xml.LoadXml(input);
                string json = Newtonsoft.Json.JsonConvert.SerializeXmlNode(
                    xml, Newtonsoft.Json.Formatting.Indented);
                return FormatResult.Ok(json);
            }
            catch (XmlException ex)
            {
                return FormatResult.Fail(XmlError(ex));
            }
        }

        private static string PrettyPrintXml(XmlDocument xml)
        {
            var settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                OmitXmlDeclaration = true
            };

            var sb = new StringBuilder();
            using (var writer = XmlWriter.Create(sb, settings))
            {
                xml.Save(writer);
            }
            return sb.ToString();
        }
    }
}
