namespace Kent.Logging.Abstractions
{
    using System;

    /// <summary>
    ///     Represents a log message.
    /// </summary>
    public class LogException
    {
        private readonly Exception _exceptionInstance;

        /// <summary>
        ///     Constructor method.
        /// </summary>
        public LogException()
        { }

        /// <summary>
        ///     Constructor method.
        /// </summary>
        /// <param name="exception">The <see cref="Exception"/> instance that represents errors.</param>
        public LogException(Exception exception)
        {
            _exceptionInstance = exception;
            Source = _exceptionInstance?.Source;
            StackTrace = _exceptionInstance?.StackTrace;
            Message = _exceptionInstance?.Message;
            InnerException = _exceptionInstance?.InnerException != null ? new LogException(_exceptionInstance.InnerException) : null;
        }

        /// <summary>
        ///     Gets or sets the name of the application or the object that causes the error.
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        ///     Gets or sets a string representation of the immediate frames on the call stack.
        /// </summary>
        public string StackTrace { get; set; }

        /// <summary>
        ///     Gets or sets a message that describes the current exception.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="LogException"/> instance that caused the current exception.
        /// </summary>
        public LogException InnerException { get; set; }

        /// <summary>
        ///     Creates and returns a string representation of the current <see cref="LogException"/>.
        /// </summary>
        public override string ToString()
        {
            return _exceptionInstance.ToString();
        }
    }
}