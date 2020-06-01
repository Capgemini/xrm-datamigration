using System;

namespace Capgemini.DataMigration.Resiliency
{
    /// <summary>
    /// Responsible for Configuring and Executing the retry.
    /// </summary>
    public interface IRetryExecutor
    {
        T Execute<T>(Func<T> p);
    }
}