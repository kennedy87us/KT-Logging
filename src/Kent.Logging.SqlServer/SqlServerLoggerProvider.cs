namespace Kent.Logging.SqlServer
{
    using Kent.Logging.SqlServer.Models;
    using Kent.SqlServer.Abstractions;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using System;
    using System.Collections.Concurrent;

    /// <summary>
    ///     A provider of <see cref="SqlServerLogger"/> instances.
    /// </summary>
    [ProviderAlias(nameof(SqlServer))]
    public sealed class SqlServerLoggerProvider : ILoggerProvider
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly ConcurrentDictionary<string, SqlServerLogger> _loggers;
        private readonly IDisposable _onChangeToken;
        private SqlServerLoggerConfiguration _currentConfig;
        private bool _disposed = false;

        /// <summary>
        ///     Constructor method.
        /// </summary>
        /// <param name="options">The options to create <see cref="SqlServerLogger"/> instances with.</param>
        /// <param name="unitOfWorkFactory">The instance of <see cref="IUnitOfWorkFactory"/>.</param>
        public SqlServerLoggerProvider(IOptionsMonitor<SqlServerLoggerConfiguration> options, IUnitOfWorkFactory<LoggingDbContext> unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _loggers = new ConcurrentDictionary<string, SqlServerLogger>(StringComparer.OrdinalIgnoreCase);
            _onChangeToken = options.OnChange(updatedConfig => _currentConfig = updatedConfig);
            _currentConfig = options.CurrentValue;
        }

        /// <summary>
        ///     Creates a new <see cref="SqlServerLogger"/> instance.
        /// </summary>
        /// <param name="categoryName">The category name for messages produced by the logger.</param>
        /// <returns>The instance of <see cref="ILogger"/> that was created.</returns>
        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => new SqlServerLogger(name, _unitOfWorkFactory, GetCurrentConfig));
        }

        /// <summary>
        ///     Releases the allocated resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Releases the allocated resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        public void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _loggers.Clear();
                    _onChangeToken.Dispose();
                }
            }
            _disposed = true;
        }

        private SqlServerLoggerConfiguration GetCurrentConfig() => _currentConfig;
    }
}