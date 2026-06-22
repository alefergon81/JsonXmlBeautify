namespace JsonXmlBeautify
{
    /// <summary>
    /// Outcome of a format operation: either success with output text, or
    /// failure with a human-readable message (including parser line/position
    /// when available).
    /// </summary>
    internal sealed class FormatResult
    {
        public bool Success { get; }
        public string Output { get; }

        private FormatResult(bool success, string output)
        {
            Success = success;
            Output = output;
        }

        public static FormatResult Ok(string output) => new(true, output);
        public static FormatResult Fail(string message) => new(false, message);
    }
}
