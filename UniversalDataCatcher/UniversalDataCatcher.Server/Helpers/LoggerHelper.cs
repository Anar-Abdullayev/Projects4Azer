using Serilog;

namespace UniversalDataCatcher.Server.Helpers
{
    public static class LoggerHelper
    {
        public static Serilog.ILogger GetLoggerConfiguration(string serviceName)
        {
            return new LoggerConfiguration()
                .WriteTo.File($"logs/info/{serviceName}/info-.log",
                rollingInterval: RollingInterval.Day,
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} " + serviceName + " - {Message:lj}{NewLine}{Exception}")
                .WriteTo.File($"logs/error/{serviceName}/error-.log",
                rollingInterval: RollingInterval.Day,
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} "+serviceName+" - {Message:lj}{NewLine}{Exception}")
                .WriteTo.Console(
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} "+serviceName+" - {Message:lj}{NewLine}{Exception}")
                .CreateLogger();
        }
    }
}
