namespace Kent.Logging.SqlServer
{
    using Kent.Logging.SqlServer.Models;
    using Kent.SqlServer.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Configuration;
    using System;

    /// <summary>
    ///     Specifies the extension methods for <see cref="ILoggingBuilder"/>.
    /// </summary>
    public static class SqlServerLoggerExtensions
    {
        /// <summary>
        ///     Adds a database logger named 'SqlServer' to the factory.
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static ILoggingBuilder AddSqlServer(this ILoggingBuilder builder)
        {
            return builder.AddSqlServer(_ => { });
        }

        /// <summary>
        ///     Adds a database logger named 'SqlServer' to the factory.
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
        /// <param name="configure">The action used to configure the <see cref="SqlServerLogger"/>.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static ILoggingBuilder AddSqlServer(this ILoggingBuilder builder, Action<SqlServerLoggerConfiguration> configure)
        {
            builder.AddConfiguration();
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, SqlServerLoggerProvider>());
            LoggerProviderOptions.RegisterProviderOptions<SqlServerLoggerConfiguration, SqlServerLoggerProvider>(builder.Services);
            builder.Services.Configure(configure);
            builder.Services.AddSqlServer<LoggingDbContext>(SqlServerLoggerUtils.GetConfigName(), null, false);
            return builder;
        }
    }
}