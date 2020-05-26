using System;
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
        public CommandAttribute(string name)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Any(x => char.IsWhiteSpace(x)))
                throw new ArgumentException("Name must not contain any white characters.", nameof(name));
            Name = name;
        }

        /// <summary>
        /// Gets the name of the current command.
        /// </summary>
        public string Name { get; }
    }
}