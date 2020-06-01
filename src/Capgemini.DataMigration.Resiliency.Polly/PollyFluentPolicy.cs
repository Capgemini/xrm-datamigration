using System;
using Polly;

namespace Capgemini.DataMigration.Resiliency.Polly
{
    public class PollyFluentPolicy : IPolicyBuilder<PollyFluentPolicy>
    {
        private PolicyBuilder p;

        public PollyFluentPolicy AddType<T>()
            where T : Exception
        {
            p = Policy.Handle<T>();
            return this;
        }

        public PollyFluentPolicy AddOrType<T>()
            where T : Exception
        {
            p = p.Or<T>();
            return this;
        }

        public virtual TResult Execute<TResult>(Func<TResult> action, int retries)
        {
            try
            {
                var res = p.Retry(retries).Execute(action);
                return res;
            }
            catch (Exception)
            {
                return default(TResult);
            }
        }
    }
}