namespace Kent.Logging.MongoDb
{
    using Kent.MongoDb.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Configuration;
    using System;

    /// <summary>
    ///     Specifies the extension methods for <see cref="ILoggingBuilder"/>.
    /// </summary>
    public static class MongoDbLoggerExtensions
    {
        /// <summary>
        ///     Adds a database logger named 'MongoDb' to the factory.
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static ILoggingBuilder AddMongoDb(this ILoggingBuilder builder)
        {
            return builder.AddMongoDb(_ => { });
        }

        /// <summary>
        ///     Adds a database logger named 'MongoDb' to the factory.
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
        /// <param name="configure">The action used to configure the <see cref="MongoDbLogger"/>.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static ILoggingBuilder AddMongoDb(this ILoggingBuilder builder, Action<MongoDbLoggerConfiguration> configure)
        {
            builder.AddConfiguration();
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, MongoDbLoggerProvider>());
            LoggerProviderOptions.RegisterProviderOptions<MongoDbLoggerConfiguration, MongoDbLoggerProvider>(builder.Services);
            builder.Services.Configure(configure);
            builder.Services.AddMongoDb(MongoDbLoggerUtils.GetConfigName());
            return builder;
        }
    }
}