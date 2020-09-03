using System;
using System.Collections.Generic;

namespace SimpleCommandLine
{
    /// <summary>
    /// Contains instances of parsed types.
    /// </summary>
    public abstract class Result
    {
        internal static Result Success(Dictionary<string, object> parsed) => new SuccessResult(parsed);
        internal static Result Error(IEnumerable<string> messages) => new ErrorResult(messages);
        public bool IsError => this is ErrorResult;
        public T GetResult<T>() where T : class
        {
            if (this is SuccessResult s)
            {
                foreach (var type in s.Parsed.Values)
                    if (type is T t) return t;
            }
            return null;
        }

        public object GetResult(string command)
        {
            if (this is SuccessResult s && s.Parsed.ContainsKey(command))
                return s.Parsed[command];
            return null;
        }

        /// <summary>
        /// Invokes a specified action if <typeparamref name="T"/> object was parsed.
        /// </summary>
        /// <typeparam name="T">Type to be evaluated.</typeparam>
        /// <param name="action">Action to be invoked.</param>
        /// <returns>The current instance.</returns>
        public Result WithParsed<T>(Action<T> action)
        {
            if (this is SuccessResult s)
                foreach (var type in s.Parsed)
                    if (type is T t) action(t);
            return this;
        }

        public IEnumerable<string> Errors => (this as ErrorResult)?.Messages;

        private class ErrorResult : Result
        {
            public IEnumerable<string> Messages { get; }
            public ErrorResult(IEnumerable<string> messages) => Messages = messages;
        }

        private class SuccessResult : Result
        {
            public Dictionary<string, object> Parsed { get; }
            internal SuccessResult(Dictionary<string, object> parsed) => Parsed = parsed;
        }
    }
}