using System;

namespace SimpleCommandLine.Tokenization.Tokens
{
    /// <summary>
    /// Represents a command-line command.
    /// </summary>
    public class CommandToken : IArgumentToken
    {
        /// <summary>
        /// Creates a new instance of <see cref="CommandToken"/>.
        /// </summary>
        /// <param name="alias">The value of this token.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="alias"/> is null.</exception>
        public CommandToken(string alias)
            => Alias = alias ?? throw new ArgumentNullException(nameof(alias));

        /// <summary>
        /// Gets this command.
        /// </summary>
        public string Alias { get; }
    }
}
