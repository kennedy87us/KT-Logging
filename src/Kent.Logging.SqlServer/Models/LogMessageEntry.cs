namespace Kent.Logging.SqlServer.Models
{
    using Kent.Logging.Abstractions;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    ///     Represents a log message entry.
    /// </summary>
    public class LogMessageEntry : LogMessage
    {
        /// <summary>
        ///     Gets or sets the id of a log entry.
        /// </summary>
        [Key]
        public override string LogId { get; set; }
    }
}