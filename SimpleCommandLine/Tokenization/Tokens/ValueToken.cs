using System;

namespace SimpleCommandLine.Tokenization.Tokens
{
    /// <summary>
    /// Represents a command-line value.
    /// </summary>
    public class ValueToken : IArgumentToken
    {
        /// <summary>
        /// Creates a new instance of <see cref="ValueToken"/>.
        /// </summary>
        /// <param name="value">The value of this argument.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
        public ValueToken(string value)
            => Value = value ?? throw new ArgumentNullException(nameof(value));

        /// <summary>
        /// Gets this value.
        /// </summary>
        public string Value { get; }

        public override string ToString() => Value;
    }
}
