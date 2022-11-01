namespace Kent.Logging.TextFile
{
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using System;
    using System.Collections.Concurrent;

    /// <summary>
    ///     A provider of <see cref="TextFileLogger"/> instances.
    /// </summary>
    [ProviderAlias(nameof(TextFile))]
    public sealed class TextFileLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, TextFileLogger> _loggers;
        private readonly IDisposable _onChangeToken;
        private TextFileLoggerConfiguration _currentConfig;
        private bool _disposed = false;

        /// <summary>
        ///     Constructor method.
        /// </summary>
        /// <param name="options">The options to create <see cref="TextFileLogger"/> instances with.</param>
        public TextFileLoggerProvider(IOptionsMonitor<TextFileLoggerConfiguration> options)
        {
            _loggers = new ConcurrentDictionary<string, TextFileLogger>(StringComparer.OrdinalIgnoreCase);
            _onChangeToken = options.OnChange(updatedConfig => _currentConfig = updatedConfig);
            _currentConfig = options.CurrentValue;
        }

        /// <summary>
        ///     Creates a new <see cref="TextFileLogger"/> instance.
        /// </summary>
        /// <param name="categoryName">The category name for messages produced by the logger.</param>
        /// <returns>The instance of <see cref="ILogger"/> that was created.</returns>
        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => new TextFileLogger(name, GetCurrentConfig));
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

        private TextFileLoggerConfiguration GetCurrentConfig() => _currentConfig;
    }
}