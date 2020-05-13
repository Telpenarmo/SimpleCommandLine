namespace SimpleCommandLine.Tokenization.Tokens
{
    /// <summary>
    /// Represents a command-line short option.
    /// </summary>
    public class ShortOptionToken : IOptionToken
    {
        /// <summary>
        /// Creates a new instance of <see cref="ShortOptionToken"/>.
        /// </summary>
        /// <param name="value">Value of the option.</param>
        public ShortOptionToken(char value) => Value = value;

        /// <summary>
        /// Gets the value of this option.
        /// </summary>
        public char Value { get; }

        public bool Equals(IArgumentToken other)
            => other is ShortOptionToken token && token.Value == Value;

        public override string ToString() => Value.ToString();
    }
}
