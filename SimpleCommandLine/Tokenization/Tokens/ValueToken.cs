using System;

namespace SimpleCommandLine.Tokenization.Tokens
{
    /// <summary>
    /// Represents a command-line atomic value.
    /// </summary>
    public class ValueToken : IValueToken
    {
        /// <summary>
        /// Creates a new instance of <see cref="ValueToken"/>.
        /// </summary>
        /// <param name="value">The value of this argument.</param>
        public ValueToken(string value) => Value = value;

        /// <summary>
        /// Gets this value.
        /// </summary>
        public string Value { get; }

        public bool Equals(IArgumentToken other)
            => other is ValueToken token ? token.Value == Value : false;

        public override string ToString() => Value;
    }
}
