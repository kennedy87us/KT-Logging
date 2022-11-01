namespace Kent.Logging.TextFile
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Configuration;
    using System;

    /// <summary>
    ///     Specifies the extension methods for <see cref="ILoggingBuilder"/>.
    /// </summary>
    public static class TextFileLoggerExtensions
    {
        /// <summary>
        ///     Adds a file logger named 'TextFile' to the factory.
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static ILoggingBuilder AddTextFile(this ILoggingBuilder builder)
        {
            return builder.AddTextFile(_ => { });
        }

        /// <summary>
        ///     Adds a file logger named 'TextFile' to the factory.
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
        /// <param name="configure">The action used to configure the <see cref="TextFileLogger"/>.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static ILoggingBuilder AddTextFile(this ILoggingBuilder builder, Action<TextFileLoggerConfiguration> configure)
        {
            builder.AddConfiguration();
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, TextFileLoggerProvider>());
            LoggerProviderOptions.RegisterProviderOptions<TextFileLoggerConfiguration, TextFileLoggerProvider>(builder.Services);
            builder.Services.Configure(configure);
            return builder;
        }
    }
}