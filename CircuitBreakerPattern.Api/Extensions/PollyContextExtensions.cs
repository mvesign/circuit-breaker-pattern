using Microsoft.Extensions.Logging;
using Polly;

namespace CircuitBreakerPattern.Api.Extensions
{
    public static class PollyContextExtensions
    {
        private const string LoggerKey = "ILogger";

        public static Context WithLogger<T>(this Context context, ILogger logger)
        {
            context[LoggerKey] = logger;
            return context;
        }

        public static ILogger GetLogger(this Context context)
        {
            return context.TryGetValue(LoggerKey, out var logger)
                ? logger as ILogger
                : null;
        }
    }
}