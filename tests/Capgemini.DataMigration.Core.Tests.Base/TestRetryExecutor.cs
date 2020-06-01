using System;
using System.Diagnostics.CodeAnalysis;
using Capgemini.DataMigration.Resiliency;

namespace Capgemini.DataMigration.Core.Tests.Base
{
    [ExcludeFromCodeCoverage]
    public class TestRetryExecutor : IRetryExecutor
    {
        public T Execute<T>(Func<T> p)
        {
            if (p == null)
            {
                return default(T);
            }

            return p.Invoke();
        }
    }
}