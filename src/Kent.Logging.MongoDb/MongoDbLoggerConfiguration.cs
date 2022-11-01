namespace Kent.Logging.MongoDb
{
    using Kent.MongoDb.Abstractions;

    /// <summary>
    ///     Configuration for <see cref="MongoDbLogger"/>.
    /// </summary>
    public class MongoDbLoggerConfiguration
    {
        /// <summary>
        ///     Defines configuration of the source.
        /// </summary>
        public MongoConfiguration Source { get; set; }
    }
}