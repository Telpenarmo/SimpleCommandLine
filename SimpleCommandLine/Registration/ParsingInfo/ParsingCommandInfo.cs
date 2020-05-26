using System;
using System.Collections.Generic;
using SimpleCommandLine.Tokenization.Tokens;

namespace SimpleCommandLine.Registration
{
    /// <summary>
    /// Encapsulates the type representing command-line command.
    /// </summary>
    internal class ParsingCommandTypeInfo : ParsingTypeInfo
    {
        /// <summary>
        /// Creates a new instance of <see cref="ParsingCommandTypeInfo"/> class.
        /// </summary>
        /// <param name="values">Collection of properties representing command-line values.</param>
        /// <param name="options">Collection of properties representing command-line options.</param>
        /// <param name="factory">Factory method that is used to create a new instance of the encapsulated type.</param>
        /// <param name="attribute">Attribute containing configuration of the command.</param>
        public ParsingCommandTypeInfo(IEnumerable<ParsingValueInfo> values, IEnumerable<ParsingOptionInfo> options, Delegate factory, CommandAttribute attribute)
            : base(values, options, factory)
        {
            Name = attribute.Name;
        }

        /// <summary>
        /// Gets all aliases of this command.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Checks if given token matches current command.
        /// </summary>
        /// <param name="token">The token to match.</param>
        /// <returns>True, if given token matches this command; false otherwise.</returns>
        public bool Match(CommandToken token) => token.Name == Name;
    }
}