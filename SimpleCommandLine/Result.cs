using System;
using System.Collections.Generic;

namespace SimpleCommandLine
{
    /// <summary>
    /// Contains instances of parsed types.
    /// </summary>
    public class Result
    {
        private readonly IEnumerable<object> parsedTypes;

        internal Result(IEnumerable<object> parsedTypes)
        {
            this.parsedTypes = parsedTypes;
        }

        /// <summary>
        /// Invokes a specified action if <typeparamref name="T"/> object was parsed.
        /// </summary>
        /// <typeparam name="T">Type to be evaluated.</typeparam>
        /// <param name="action">Action to be invoked.</param>
        /// <returns>The current instance.</returns>
        public Result WithParsed<T>(Action<T> action)
        {
            foreach (var type in parsedTypes)
                if (type is T parsed)
                    action(parsed);
            return this;
        }
    }
}