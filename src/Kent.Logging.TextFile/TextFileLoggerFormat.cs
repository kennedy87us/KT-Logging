namespace Kent.Logging.TextFile
{
    /// <summary>
    ///     Format of <see cref="TextFileLogger"/> messages.
    /// </summary>
    public enum TextFileLoggerFormat
    {
        /// <summary>
        ///     Produces messages in the default format (single line).
        /// </summary>
        Default,

        /// <summary>
        ///     Produces messages in multiple lines.
        /// </summary>
        MultiLines
    }
}