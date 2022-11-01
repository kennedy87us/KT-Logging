namespace Kent.Logging.Tests.Infrastructure
{
    using Microsoft.Extensions.Options;
    using System;

    public class TestOptionsMonitor<TOptions> : IOptionsMonitor<TOptions> where TOptions : class
    {
        public TestOptionsMonitor(TOptions currentValue)
        {
            CurrentValue = currentValue;
        }

        public TOptions Get(string name)
        {
            return CurrentValue;
        }

        public IDisposable OnChange(Action<TOptions, string> listener)
        {
            listener?.Invoke(CurrentValue, String.Empty);
            return null;
        }

        public TOptions CurrentValue { get; }
    }
}