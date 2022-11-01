namespace Kent.Logging.MongoDb
{
    using Kent.Logging.Abstractions;
    using Kent.Logging.MongoDb.Models;
    using Kent.MongoDb.Abstractions;
    using Microsoft.Extensions.Logging;
    using System;

    /// <summary>
    ///     Represents a typed used to peform logging to MongoDb.
    /// </summary>
    public class MongoDbLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly IMongoContextFactory _mongoContextFactory;
        private readonly Func<MongoDbLoggerConfiguration> _getCurrentConfig;

        /// <summary>
        ///     Constructor method.
        /// </summary>
        /// <param name="categoryName">The category name for messages produced by the logger.</param>
        /// <param name="mongoContextFactory">The instance of <see cref="IMongoContextFactory"/>.</param>
        /// <param name="getCurrentConfig">The function used to get the configuration.</param>
        public MongoDbLogger(string categoryName, IMongoContextFactory mongoContextFactory, Func<MongoDbLoggerConfiguration> getCurrentConfig)
        {
            (_categoryName, _mongoContextFactory, _getCurrentConfig) = (categoryName, mongoContextFactory, getCurrentConfig);
        }

        /// <summary>
        ///     Configuration for <see cref="MongoDbLogger"/>.
        /// </summary>
        public MongoDbLoggerConfiguration Configuration { get; set; }

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
                    throw new ArgumentNullException("formatter");
                }

                string message = formatter(state, exception);
                if (!string.IsNullOrEmpty(message) || exception != null)
                {
                    WriteMessage(logLevel, _categoryName, eventId.Id, message, exception);
                }
            }
        }

        /// <summary>
        ///     Peforms logging the message to MongoDb.
        /// </summary>
        /// <param name="logLevel">The entry will be written on this level.</param>
        /// <param name="categoryName">The category log name.</param>
        /// <param name="eventId">The logging event id.</param>
        /// <param name="message">The entry message to be written.</param>
        /// <param name="exception">The exception related to this entry.</param>
        protected virtual void WriteMessage(LogLevel logLevel, string categoryName, int eventId, string message, Exception exception)
        {
            var logEntry = new LogMessageEntry()
            {
                LogId = Guid.NewGuid().ToString(),
                CreatedDate = DateTime.Now,
                LogLevel = logLevel,
                CategoryName = categoryName,
                EventId = eventId,
                Message = message,
                Exception = exception != null ? new LogException(exception) : null
            };

            var mongoContex = _mongoContextFactory.CreateContext(MongoDbLoggerUtils.GetConfigName());
            mongoContex.GetCollection<LogMessageEntry>().InsertOne(logEntry);
        }
    }
}