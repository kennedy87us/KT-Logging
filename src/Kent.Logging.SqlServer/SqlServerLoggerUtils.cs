namespace Kent.Logging.SqlServer
{
    /// <summary>
    ///     Utilities for <see cref="SqlServerLogger"/>.
    /// </summary>
    public static class SqlServerLoggerUtils
    {
        /// <summary>
        ///     Gets configuration name being used.
        /// </summary>
        /// <returns>A string represents the configuration name.</returns>
        public static string GetConfigName()
        {
            return $"{nameof(Logging)}:{nameof(SqlServer)}:{nameof(SqlServerLoggerConfiguration.Source)}";
        }
    }
}