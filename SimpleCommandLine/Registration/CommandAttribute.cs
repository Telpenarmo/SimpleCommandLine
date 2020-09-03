using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleCommandLine
{
    /// <summary>
    /// Marks the type as a command-line command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class CommandAttribute : Attribute
    {
        /// <summary>
        /// Initializes the <see cref="CommandAttribute"/> with at least one alias.
        /// </summary>
        /// <param name="name">Array of aliases.</param>
        /// <exception cref="ArgumentException">Thrown when no alias is provided or any alias is invalid.</exception>
        public CommandAttribute(string name, params string[] aliases)
        {
            var names = new string[aliases.Length + 1];
            names[0] =  name;
            Array.Copy(aliases, 0, names, 1, aliases.Length);
            if (names.Any(alias => string.IsNullOrWhiteSpace(alias) || alias.Any(x => char.IsWhiteSpace(x))))
                throw new ArgumentException("Name must not contain any white characters.");
            Aliases = names;
        }

        /// <summary>
        /// Gets the name of the current command.
        /// </summary>
        public IEnumerable<string> Aliases { get; }
    }
}