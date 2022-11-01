namespace Kent.Logging.TextFile
{
    /// <summary>
    ///     Configuration for <see cref="TextFileLogger"/>.
    /// </summary>
    public class TextFileLoggerConfiguration
    {
        /// <summary>
        ///     Defines the log file path, it can be either relative or absolute.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        ///     Defines the option to use local path for the log file.
        ///     This will be ignored if <see cref="TextFileLoggerConfiguration.FilePath"/> is a absolute path.
        /// </summary>
        public bool UseLocalPath { get; set; }

        /// <summary>
        ///     Defines the format used for the log file name. Defaults to null meant no log produced.
        /// </summary>
        public string FileNameFormat { get; set; }

        /// <summary>
        ///     Defines the format string used to format timestamp in logging messages. Defaults to null.
        /// </summary>
        public string TimestampFormat { get; set; }

        /// <summary>
        ///     Defines the option to include the category log name in logging messages.
        /// </summary>
        public bool IncludeCategory { get; set; }

        /// <summary>
        ///     Defines log message format. Defaults to <see cref="TextFileLoggerFormat.Default"/>.
        /// </summary>
        public TextFileLoggerFormat Format { get; set; } = TextFileLoggerFormat.Default;
    }
}