using System;

namespace Capgemini.DataMigration.Resiliency
{
    /// <summary>
    /// A Fluent way to generate a retry Policy Builder.
    /// </summary>
    public interface IPolicyBuilder<TType>
    {
        /// <summary>
        /// Initialises the fluent policy builder.
        /// </summary>
        /// <typeparam name="T">T.</typeparam>
        /// <returns>return.</returns>
        TType AddType<T>()
            where T : Exception;

        /// <summary>
        /// Adds n number of Exception types allowing you to chain and build a complex command.
        /// </summary>
        /// <typeparam name="T">T.</typeparam>
        /// <returns>return.</returns>
        TType AddOrType<T>()
            where T : Exception;

        /// <summary>
        /// Executes an anaoymous function and returns a value.
        /// </summary>
        /// <typeparam name="TResult">TResult.</typeparam>
        /// <param name="action">action.</param>
        /// <param name="retries">retries.</param>
        /// <returns>return.</returns>
        TResult Execute<TResult>(Func<TResult> action, int retries);
    }
}