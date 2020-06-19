namespace SimpleCommandLine.Tokens
{
    /// <summary>
    /// Represents a command-line argument.
    /// </summary>
    /// <seealso cref="IOptionToken"/>
    /// <seealso cref="ValueToken"/>
    public interface IArgumentToken : System.IEquatable<IArgumentToken> { }
}