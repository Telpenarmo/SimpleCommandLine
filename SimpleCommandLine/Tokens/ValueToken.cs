namespace SimpleCommandLine.Tokens
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
        public ValueToken(string value) => Value = value;

        /// <summary>
        /// Gets this value.
        /// </summary>
        public string Value { get; }

        public virtual bool Equals(IArgumentToken other)
            => other is ValueToken token && token.Value == Value;

        public override string ToString() => Value;
    }
}
