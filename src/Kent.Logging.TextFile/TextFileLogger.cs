namespace Kent.Logging.TextFile
{
    using Kent.Logging.Abstractions;
    using Microsoft.Extensions.Logging;
    using System;
    using System.IO;
    using System.Reflection;
    using System.Text;

    /// <summary>
    ///     Represents a typed used to peform logging to a text file.
    /// </summary>
    public class TextFileLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly string _messagePadding;
        private readonly Func<TextFileLoggerConfiguration> _getCurrentConfig;

        /// <summary>
        ///     Constructor method.
        /// </summary>
        /// <param name="categoryName">The category name for messages produced by the logger.</param>
        /// <param name="getCurrentConfig">The function used to get the configuration.</param>
        public TextFileLogger(string categoryName, Func<TextFileLoggerConfiguration> getCurrentConfig)
        {
            (_categoryName, _getCurrentConfig) = (categoryName, getCurrentConfig);
            _messagePadding = new string(' ', 4);
        }

        /// <summary>
        ///     Configuration for <see cref="TextFileLogger"/>.
        /// </summary>
        public TextFileLoggerConfiguration Configuration { get; set; }

        /// <summary>
        ///     Begins a logical operation scope.
        /// </summary>
        /// <typeparam name="TState">The type of the state to begin scope for.</typeparam>
        /// <param name="state">The identifier for the scope.</param>
        /// <returns>An <see cref="IDisposable"/> that ends the logical operation scope on dispose.</returns>
        public IDisposable BeginScope<TState>(TState state) => default!;

        /// <summary>
        ///     Checks if the given log level is enabled.
        /// </summary>
        /// <param name="logLevel">The log level to be checked.</param>
        /// <returns>true if enabled.</returns>
        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

        /// <summary>
        ///     Writes a log entry.
        /// </summary>
        /// <typeparam name="TState">The type of the object to be written.</typeparam>
        /// <param name="logLevel">The entry will be written on this level.</param>
        /// <param name="eventId">The logging event.</param>
        /// <param name="state">The entry to be written. Can be also an object.</param>
        /// <param name="exception">The exception related to this entry.</param>
        /// <param name="formatter">The function to create a message of the state and exception.</param>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            Configuration = _getCurrentConfig();

            if (IsEnabled(logLevel))
            {
                if (formatter == null)
                {
                    throw new ArgumentNullException(nameof(formatter));
                }

                string message = formatter(state, exception);
                if (!string.IsNullOrEmpty(message) || exception != null)
                {
                    WriteMessage(logLevel, _categoryName, eventId.Id, message, exception);
                }
            }
        }

        /// <summary>
        ///     Peforms logging the message to a text file.
        /// </summary>
        /// <param name="logLevel">The entry will be written on this level.</param>
        /// <param name="categoryName">The category log name.</param>
        /// <param name="eventId">The logging event id.</param>
        /// <param name="message">The entry message to be written.</param>
        /// <param name="exception">The exception related to this entry.</param>
        protected virtual void WriteMessage(LogLevel logLevel, string categoryName, int eventId, string message, Exception exception)
        {
            var filename = ParseFileName(Configuration.FileNameFormat);
            var filePath = CreateDirectory(Configuration.FilePath, Configuration.UseLocalPath);
            var fullFilePath = Path.Combine(filePath, filename);

            if (!string.IsNullOrEmpty(fullFilePath))
            {
                var logPlainText = string.Empty;
                var logMessage = new LogMessage()
                {
                    CreatedDate = DateTime.Now,
                    LogLevel = logLevel,
                    CategoryName = categoryName,
                    EventId = eventId,
                    Message = message,
                    Exception = exception != null ? new LogException(exception) : null
                };

                switch (Configuration.Format)
                {
                    case TextFileLoggerFormat.Default:
                        logPlainText = CreateDefaultLogMessage(logMessage);
                        break;
                    case TextFileLoggerFormat.MultiLines:
                        logPlainText = CreateMultiLinesLogMessage(logMessage);
                        break;
                    default:
                        break;
                }

                // the Dispose method automatically flushes and closes the stream
                using StreamWriter streamWriter = new(fullFilePath, true);
                streamWriter.WriteLine(logPlainText);
            }
            else
            {
                throw new InvalidOperationException("Target full file path is invalid");
            }
        }

        private string CreateDefaultLogMessage(LogMessage logMessage)
        {
            var logBuilder = new StringBuilder();
            var timestampFormat = Configuration.TimestampFormat;

            logBuilder.Append(!string.IsNullOrWhiteSpace(timestampFormat) ? $"{logMessage.CreatedDate.ToString(timestampFormat)}" : $"{logMessage.CreatedDate}");
            logBuilder.Append("> [");
            logBuilder.Append(GetLogLevelString(logMessage.LogLevel));
            logBuilder.Append(Configuration.IncludeCategory ? $" - {logMessage.CategoryName}" : null);
            logBuilder.Append("]");
            logBuilder.Append($" {logMessage.Message}");

            if (logMessage.Exception != null)
            {
                logBuilder.AppendLine();
                logBuilder.Append(_messagePadding);
                logBuilder.Append($" {logMessage.Exception}");
            }

            return logBuilder.ToString();
        }

        private string CreateMultiLinesLogMessage(LogMessage logMessage)
        {
            var logBuilder = new StringBuilder();
            var timestampFormat = Configuration.TimestampFormat;

            logBuilder.Append(!string.IsNullOrWhiteSpace(timestampFormat) ? $"{logMessage.CreatedDate.ToString(timestampFormat)}" : $"{logMessage.CreatedDate}");
            logBuilder.Append("> [");
            logBuilder.Append(GetLogLevelString(logMessage.LogLevel));
            logBuilder.Append(Configuration.IncludeCategory ? $" - {logMessage.CategoryName}" : null);
            logBuilder.Append("]");

            if (!string.IsNullOrEmpty(logMessage.Message))
            {
                logBuilder.AppendLine();
                logBuilder.Append(_messagePadding);
                logBuilder.Append($" {logMessage.Message}");
            }

            if (logMessage.Exception != null)
            {
                logBuilder.AppendLine();
                logBuilder.Append(_messagePadding);
                logBuilder.Append($" {logMessage.Exception}");
            }

            return logBuilder.ToString();
        }

        private static string GetLogLevelString(LogLevel logLevel) => logLevel.ToString().ToUpper();

        private static string ParseFileName(string filename)
        {
            if (!string.IsNullOrWhiteSpace(filename))
            {
                //Replace datetime format in log file name
                int posStart = filename.IndexOf('%');
                int posEnd = filename.LastIndexOf('%');
                string dateTimeFormat = filename.Substring(posStart + 1, posEnd - posStart).Replace("%", string.Empty);
                if (!string.IsNullOrEmpty(dateTimeFormat))
                {
                    filename = filename.Replace(dateTimeFormat, DateTime.Now.ToString(dateTimeFormat));
                }
                filename = filename.Replace("%", string.Empty);
            }
            return filename;
        }

        private static string CreateDirectory(string filePath, bool useLocalPath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                try
                {
                    if (!Path.IsPathFullyQualified(filePath))
                    {
                        if (useLocalPath)
                        {
                            var assembly = Assembly.GetEntryAssembly();
                            string subFolder = assembly.GetName().Name;
                            object[] attributes = assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                            if (attributes.Length > 0)
                            {
                                subFolder = ((AssemblyCompanyAttribute)attributes[0]).Company;
                            }

                            filePath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), subFolder, filePath);
                        }
                    }
                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return filePath;
        }
    }
}