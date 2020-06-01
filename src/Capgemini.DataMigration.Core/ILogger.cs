using System;

namespace Capgemini.DataMigration.Core
{
    /// <summary>
    /// Logging interface.
    /// </summary>
    public interface ILogger
    {
        void LogError(string message);

        void LogError(string message, Exception ex);

        void LogInfo(string message);

        void LogVerbose(string message);

        void LogWarning(string message);
    }
}