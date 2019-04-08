using Capgemini.DataMigration.Core;
using System;
using System.Collections.Generic;

namespace Capgemini.Xrm.Datamigration.Examples
{
        public class ConsoleLogger : ILogger
        {
            /// <summary>
            /// LogLevel
            /// 0 - only Errors
            /// 1 - Warnings
            /// 2 - Info
            /// 3 - Verbose
            /// </summary>
            public static int LogLevel { get; set; } = 2;

            public void Error(string message)
            {
                Console.WriteLine("Error:" + message);
            }

            public void Error(string message, Exception ex)
            {
                Console.WriteLine("Error:" + message + ",Ex:" + ex.ToString());
            }

            public void Info(string message)
            {
                if (LogLevel > 1)
                    Console.WriteLine("Info:" + message);
            }

            public void Verbose(string message)
            {
                if (LogLevel > 2)
                    Console.WriteLine("Verbose:" + message);
            }

            public void Warning(string message)
            {
                if (LogLevel > 0)
                    Console.WriteLine("Warning:" + message);
            }
        }
    }
