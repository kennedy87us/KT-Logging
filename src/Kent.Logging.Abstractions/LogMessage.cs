namespace Kent.Logging.Abstractions
{
    using Microsoft.Extensions.Logging;
    using System;

    /// <summary>
    ///     Represents a log message.
    /// </summary>
    public class LogMessage
    {
        /// <summary>
        ///     Gets or sets the id of a log entry.
        /// </summary>
        public virtual string LogId { get; set; }

        /// <summary>
        ///     Gets or sets logging creation date.
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        ///     Gets or sets the logging level.
        /// </summary>
        public LogLevel LogLevel { get; set; }

        /// <summary>
        ///     Gets or sets the category name.
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        ///     Gets or sets the logging event.
        /// </summary>
        public int EventId { get; set; }

        /// <summary>
        ///     Gets or sets the logging message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        ///     Gets or sets the logging exception.
        /// </summary>
        public LogException Exception { get; set; }
    }
}