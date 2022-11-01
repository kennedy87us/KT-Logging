namespace Kent.Logging.SqlServer
{
    using Kent.SqlServer.Abstractions;

    /// <summary>
    ///     Configuration for <see cref="SqlServerLogger"/>.
    /// </summary>
    public class SqlServerLoggerConfiguration
    {
        /// <summary>
        ///     Defines configuration of the source.
        /// </summary>
        public SqlConfiguration Source { get; set; }

        /// <summary>
        ///     Defines the option to create logging table if not exists.
        /// </summary>
        public bool EnsureTableCreated { get; set; }
    }
}