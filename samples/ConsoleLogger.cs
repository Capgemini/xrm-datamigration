using System;
using System.Diagnostics.CodeAnalysis;
using Capgemini.DataMigration.Core;

namespace Capgemini.Xrm.Datamigration.Examples
{
    [ExcludeFromCodeCoverage]
    public class ConsoleLogger : ILogger
    {
        /// <summary>
        /// Gets or sets LogLevel - 0 - only Errors, 1 - Warnings, 2 - Info, 3 - Verbose.
        /// </summary>
        public static int LogLevel { get; set; } = 2;

        public void LogError(string message)
        {
            Console.WriteLine("Error:" + message);
        }

        public void LogError(string message, Exception ex)
        {
            Console.WriteLine($"Error: {message} ,Ex: {ex}");
        }

        public void LogInfo(string message)
        {
            if (LogLevel > 1)
            {
                Console.WriteLine($"Info: {message}");
            }
        }

        public void LogVerbose(string message)
        {
            if (LogLevel > 2)
            {
                Console.WriteLine($"Verbose: {message}");
            }
        }

        public void LogWarning(string message)
        {
            if (LogLevel > 0)
            {
                Console.WriteLine($"Warning: {message}");
            }
        }
    }
}