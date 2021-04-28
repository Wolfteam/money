using Microsoft.Extensions.Hosting;
using Money.Api.Models.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Filters;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Collections.Generic;
using System.IO;

namespace Money.Api.Extensions
{
    public static class LoggingExtensions
    {
        private const string LoggingFormat = "{Timestamp:dd-MM-yyyy HH:mm:ss.fff} ({ThreadId}) [{Level}] {Message:lj}{NewLine}{Exception}";

        public static void SetupLogging(this List<FileToLog> logs, string logsPath = null)
        {
            string basePath = logsPath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");

            if (string.IsNullOrWhiteSpace(basePath))
                throw new ArgumentNullException(nameof(logsPath), "You need to provide a base path for the logs");

            var loggingConfig = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
                .Enrich.WithThreadId()
                .Enrich.FromLogContext()
                .WriteTo.Debug(outputTemplate: LoggingFormat)
                .WriteTo.Console(outputTemplate: LoggingFormat, theme: AnsiConsoleTheme.Literate);

            foreach (var kvp in logs)
            {
                string filename = $"{kvp.FileName}_.txt";
                if (kvp.Filtered)
                {
                    loggingConfig.WriteTo
                        .Logger(l => l.Filter.ByIncludingOnly(Matching.FromSource(kvp.AssemblyFullName))
                            .WriteTo.File(
                                Path.Combine(basePath, filename),
                                rollingInterval: RollingInterval.Day,
                                rollOnFileSizeLimit: true,
                                outputTemplate: LoggingFormat));
                }
                else
                {
                    loggingConfig.WriteTo
                        .Logger(l => l
                            .WriteTo.File(
                                Path.Combine(basePath, filename),
                                rollingInterval: RollingInterval.Day,
                                rollOnFileSizeLimit: true,
                                outputTemplate: LoggingFormat));
                }
            }
            Log.Logger = loggingConfig.CreateLogger();
        }

        public static IHostBuilder ConfigureAppLogging(this IHostBuilder builder) => builder.UseSerilog();
    }
}
