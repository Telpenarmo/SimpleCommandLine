using System;
using System.Collections.Generic;

namespace SimpleCommandLine
{
    /// <summary>
    /// Provides methods to access and operate on instances of parsed objects.
    /// </summary>
    public abstract class Result
    {
        internal static Result Success(Dictionary<string, object> parsed) => new SuccessResult(parsed);
        internal static Result Error(IEnumerable<string> messages) => new ErrorResult(messages);

        /// <summary>
        /// Indicates if parsing proccess failed.
        /// </summary>
        public bool IsError => this is ErrorResult;

        /// <summary>
        /// Returns object of <typeparamref name="T"/> type if it was parsed.
        /// </summary>
        /// <typeparam name="T">Type of an object to return.</typeparam>
        /// <returns>Parsed object.</returns>
        /// <remarks>If more than one object of <typeparamref name="T"/> was parsed, result is chosen ambigously. 
        /// To have a control over which object to return, use <see cref="Result.GetResult{T}(string)"/>.</remarks>
        public T? GetResult<T>() where T : class
        {
            if (this is SuccessResult s)
            {
                foreach (var type in s.Parsed.Values)
                    if (type is T t) return t;
            }
            return null;
        }

        /// <summary>
        /// Returns object of <typeparamref name="T"/> type specified by <paramref name="command"/> if it was parsed.
        /// </summary>
        /// <typeparam name="T">Type of an object to return.</typeparam>
        /// <param name="command">String entered by the user representing a command.</param>
        /// <returns>Parsed object.</returns>
        public T? GetResult<T>(string command) where T : class
            => this is SuccessResult s && s.Parsed.ContainsKey(command) ? s.Parsed[command] as T : null;

        /// <summary>
        /// Invokes a specified action if <typeparamref name="T"/> object was parsed.
        /// </summary>
        /// <typeparam name="T">Type to be evaluated.</typeparam>
        /// <param name="action">Action to be invoked.</param>
        /// <returns>The current instance.</returns>
        public Result WithParsed<T>(Action<T> action)
        {
            if (this is SuccessResult s)
                foreach (var type in s.Parsed.Values)
                    if (type is T t) action(t);
            return this;
        }

        /// <summary>
        /// Invokes a specified action if <typeparamref name="T"/> object identified by <paramref name="command"/> was parsed.
        /// </summary>
        /// <typeparam name="T">Type to be evaluated.</typeparam>
        /// <param name="action">Action to be invoked.</param>
        /// <param name="command">String entered by the user representing a command.</param>
        /// <returns>The current instance.</returns>
        public Result WithParsed<T>(Action<T> action, string command)
        {
            if (this is SuccessResult s && s.Parsed.TryGetValue(command, out var o) && o is T t)
                action(t);
            return this;
        }

        /// <summary>
        /// If any errors occured during parsing proccess, returns the messages.
        /// </summary>
        /// <returns>Collection containing error messages if <see cref="IsError"/> is true; null otherwise.</returns>
        public IEnumerable<string>? Errors => (this as ErrorResult)?.Messages;

        private class ErrorResult : Result
        {
            public IEnumerable<string> Messages { get; }
            public ErrorResult(IEnumerable<string> messages) => Messages = messages;
        }

        private class SuccessResult : Result
        {
            public Dictionary<string, object> Parsed { get; }
            public SuccessResult(Dictionary<string, object> parsed) => Parsed = parsed;
        }
    }
}