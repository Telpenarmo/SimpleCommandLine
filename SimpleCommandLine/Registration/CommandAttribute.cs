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
        private readonly List<string> aliases = new List<string>();

        /// <summary>
        /// Initializes the <see cref="CommandAttribute"/> with at least one alias.
        /// </summary>
        /// <param name="name">Array of aliases.</param>
        /// <exception cref="ArgumentException">Thrown when no alias is provided or any alias is invalid.</exception>
        public CommandAttribute(params string[] aliases)
        {
            if (aliases.Length == 0)
                throw new ArgumentException("At least one name must be provided.");
            this.aliases.AddRange(aliases.Where(s => CheckName(s)));
        }

        /// <summary>
        /// Gets all aliases of the current command.
        /// </summary>
        public IEnumerable<string> Aliases => aliases;

        private bool CheckName(string alias)
        {
            if (string.IsNullOrWhiteSpace(alias) || alias.Any(x => char.IsWhiteSpace(x)))
                throw new ArgumentException("Name must not contain any white characters.", nameof(alias));
            else
                return true;
        }
    }
}