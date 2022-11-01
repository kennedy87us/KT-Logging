namespace Kent.Logging.MongoDb.Models
{
    using Kent.Logging.Abstractions;
    using MongoDB.Bson.Serialization.Attributes;

    /// <summary>
    ///     Represents a log message entry.
    /// </summary>
    public class LogMessageEntry : LogMessage
    {
        /// <summary>
        ///     Gets or sets the id of a log entry.
        /// </summary>
        [BsonId]
        public override string LogId { get; set; }
    }
}