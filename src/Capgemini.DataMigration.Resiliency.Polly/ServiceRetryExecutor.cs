using System;
using System.ServiceModel;

namespace Capgemini.DataMigration.Resiliency.Polly
{
    /// <summary>
    /// A Facade wrapper around the executor.
    /// </summary>
    public class ServiceRetryExecutor : IRetryExecutor
    {
        private readonly IPolicyBuilder<PollyFluentPolicy> builder;

        public ServiceRetryExecutor()
            : this(new PollyFluentPolicy())
        {
        }

        public ServiceRetryExecutor(IPolicyBuilder<PollyFluentPolicy> builder)
        {
            this.builder = builder;
        }

        public T Execute<T>(Func<T> p)
        {
            return builder
                    .AddType<TimeoutException>()
                    .AddOrType<FaultException>().Execute(p, 5);
        }
    }
}