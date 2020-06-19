using System;
using System.Collections.Generic;

namespace SimpleCommandLine
{
    /// <summary>
    /// Contains instances of parsed types.
    /// </summary>
    public class Result
    {
        private readonly IEnumerable<object> parsed;
        public IEnumerable<string> Errors { get; }

        internal Result(IEnumerable<object> parsed) => this.parsed = parsed;
        public Result(IEnumerable<string> errors) => Errors = errors;

        public T GetResult<T>()
        {
            foreach (var type in parsed)
                if (type is T t)
                    return t;
            return default;
        }
    }
}