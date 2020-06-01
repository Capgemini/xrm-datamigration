using System;
using System.Diagnostics;

namespace Capgemini.DataMigration.Core
{
    public class ConsoleLogger : ILogger
    {
        /// <summary>
        /// Gets or sets logLevel
        /// 0 - only Errors
        /// 1 - Warnings
        /// 2 - Info
        /// 3 - Verbose.
        /// </summary>
        public static int LogLevel { get; set; } = 2;

        public void LogError(string message)
        {
            Trace.WriteLine($"Error:{message}");
        }

        public void LogError(string message, Exception ex)
        {
            Trace.WriteLine($"Error:{message},Ex:{ex?.ToString()}");
        }

        public void LogInfo(string message)
        {
            if (LogLevel > 1)
            {
                Trace.WriteLine($"Info:{message}");
            }
        }

        public void LogVerbose(string message)
        {
            if (LogLevel > 2)
            {
                Trace.WriteLine($"Verbose:{message}");
            }
        }

        public void LogWarning(string message)
        {
            if (LogLevel > 0)
            {
                Trace.WriteLine($"Warning:{message}");
            }
        }
    }
}