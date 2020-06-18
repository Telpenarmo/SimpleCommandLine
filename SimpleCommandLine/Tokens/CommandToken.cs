using System;

namespace SimpleCommandLine.Tokens
{
    /// <summary>
    /// Represents a command-line command.
    /// </summary>
    public class CommandToken : IArgumentToken
    {
        /// <summary>
        /// Creates a new instance of <see cref="CommandToken"/>.
        /// </summary>
        /// <param name="name">The value of this token.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is null.</exception>
        public CommandToken(string name)
            => Name = name ?? throw new ArgumentNullException(nameof(name));

        /// <summary>
        /// Gets this command.
        /// </summary>
        public string Name { get; }

        public bool Equals(IArgumentToken other)
            => other is CommandToken token && token.Name == Name;
    }
}