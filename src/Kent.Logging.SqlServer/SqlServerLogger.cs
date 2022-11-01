namespace Kent.Logging.SqlServer
{
    using Kent.Logging.Abstractions;
    using Kent.Logging.SqlServer.Models;
    using Kent.SqlServer.Abstractions;
    using Microsoft.Extensions.Logging;
    using System;
    using System.IO;

    /// <summary>
    ///     Represents a typed used to peform logging to SQL Server.
    /// </summary>
    public class SqlServerLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly Func<SqlServerLoggerConfiguration> _getCurrentConfig;

        private static readonly object _lockObject = new object();
        private const string CREATE_TABLE_SCRIPT = "CreateTableIfNotExist.sql";

        /// <summary>
        ///     Constructor method.
        /// </summary>
        /// <param name="categoryName">The category name for messages produced by the logger.</param>
        /// <param name="unitOfWorkFactory">The instance of <see cref="IUnitOfWorkFactory"/>.</param>
        /// <param name="getCurrentConfig">The function used to get the configuration.</param>
        public SqlServerLogger(string categoryName, IUnitOfWorkFactory unitOfWorkFactory, Func<SqlServerLoggerConfiguration> getCurrentConfig)
        {
            (_categoryName, _unitOfWorkFactory, _getCurrentConfig) = (categoryName, unitOfWorkFactory, getCurrentConfig);
        }

        /// <summary>
        ///     Configuration for <see cref="SqlServerLogger"/>.
        /// </summary>
        public SqlServerLoggerConfiguration Configuration { get; set; }

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
        ///     Peforms logging the message to SQL Server.
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

            using (var unitOfWork = _unitOfWorkFactory.CreateUnitOfWork())
            {
                if (Configuration.EnsureTableCreated)
                {
                    lock (_lockObject)
                    {
                        var result = unitOfWork.ExecuteCommand(GetContentFromAssembly<SqlServerLogger>($"Scripts.{CREATE_TABLE_SCRIPT}"))
                                               .GetAwaiter().GetResult();
                        if (Convert.ToBoolean(result) is bool isTableCreated) Configuration.EnsureTableCreated = false;
                    }
                }

                var repository = unitOfWork.GetEntityRepository<LogMessageEntry>();
                repository.InsertOne(logEntry);
                unitOfWork.Save();
            }
        }

        private string GetContentFromAssembly<TType>(string resourceName)
        {
            var result = string.Empty;

            using (var stream = typeof(TType).Assembly.GetManifestResourceStream($"{typeof(TType).Namespace}.{resourceName}"))
            {
                using (var reader = new StreamReader(stream, leaveOpen: true))
                {
                    result = reader.ReadToEnd();
                }
            }

            return result;
        }
    }
}