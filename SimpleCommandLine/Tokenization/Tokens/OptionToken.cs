using System;

namespace SimpleCommandLine.Tokens
{
    /// <summary>
    /// Represents a command-line long option.
    /// </summary>
    public class OptionToken : IArgumentToken
    {
        /// <summary>
        /// Creates a new instance of <see cref="LongOptionToken"/>.
        /// </summary>
        /// <param name="value">Value of the option.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
        public OptionToken(string value)
            => Value = value ?? throw new ArgumentNullException(nameof(value));

        /// <summary>
        /// Gets the value of this option.
        /// </summary>
        public string Value { get; }

        public bool Equals(IArgumentToken other)
            => other is OptionToken token && token.Value == Value;

        public override string ToString() => Value;
    }
}