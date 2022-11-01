namespace Kent.Logging.MongoDb
{
    using Kent.MongoDb.Abstractions;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using System;
    using System.Collections.Concurrent;

    /// <summary>
    ///     A provider of <see cref="MongoDbLogger"/> instances.
    /// </summary>
    [ProviderAlias(nameof(MongoDb))]
    public sealed class MongoDbLoggerProvider : ILoggerProvider
    {
        private readonly IMongoContextFactory _mongoContextFactory;
        private readonly ConcurrentDictionary<string, MongoDbLogger> _loggers;
        private readonly IDisposable _onChangeToken;
        private MongoDbLoggerConfiguration _currentConfig;
        private bool _disposed = false;

        /// <summary>
        ///     Constructor method.
        /// </summary>
        /// <param name="options">The options to create <see cref="MongoDbLogger"/> instances with.</param>
        /// <param name="mongoContextFactory">The instance of <see cref="IMongoContextFactory"/>.</param>
        public MongoDbLoggerProvider(IOptionsMonitor<MongoDbLoggerConfiguration> options, IMongoContextFactory mongoContextFactory)
        {
            _mongoContextFactory = mongoContextFactory;
            _loggers = new ConcurrentDictionary<string, MongoDbLogger>(StringComparer.OrdinalIgnoreCase);
            _onChangeToken = options.OnChange(updatedConfig => _currentConfig = updatedConfig);
            _currentConfig = options.CurrentValue;
        }

        /// <summary>
        ///     Creates a new <see cref="MongoDbLogger"/> instance.
        /// </summary>
        /// <param name="categoryName">The category name for messages produced by the logger.</param>
        /// <returns>The instance of <see cref="ILogger"/> that was created.</returns>
        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => new MongoDbLogger(name, _mongoContextFactory, GetCurrentConfig));
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

        private MongoDbLoggerConfiguration GetCurrentConfig() => _currentConfig;
    }
}