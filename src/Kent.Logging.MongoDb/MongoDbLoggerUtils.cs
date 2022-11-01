namespace Kent.Logging.MongoDb
{
    /// <summary>
    ///     Utilities for <see cref="MongoDbLogger"/>.
    /// </summary>
    public static class MongoDbLoggerUtils
    {
        /// <summary>
        ///     Gets configuration name being used.
        /// </summary>
        /// <returns>A string represents the configuration name.</returns>
        public static string GetConfigName()
        {
            return $"{nameof(Logging)}:{nameof(MongoDb)}:{nameof(MongoDbLoggerConfiguration.Source)}";
        }
    }
}