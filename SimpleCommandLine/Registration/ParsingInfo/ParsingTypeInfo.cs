using System;
using System.Collections.Generic;
using System.Linq;
using SimpleCommandLine.Tokenization.Tokens;

namespace SimpleCommandLine.Registration
{
    /// <summary>
    /// Encapsulates the type.
    /// </summary>
    internal class ParsingTypeInfo
    {
        /// <summary>
        /// Creates a new instance od <see cref="ParsingTypeInfo"/> class.
        /// </summary>
        /// <param name="values">Collection of properties representing command-line values.</param>
        /// <param name="options">Collection of properties representing command-line options.</param>
        /// <param name="factory">Factory method that is used to create a new instance of the encapsulated type.</param>
        public ParsingTypeInfo(IEnumerable<ParsingValueInfo> values, IEnumerable<ParsingOptionInfo> options, Delegate factory)
        {
            Values = values.OrderBy(x => x.Index) ?? throw new ArgumentNullException(nameof(values));
            Options = options ?? throw new ArgumentNullException(nameof(options));
            Factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }
        
        internal IEnumerable<ParsingValueInfo> Values { get; }        
        internal IEnumerable<ParsingOptionInfo> Options { get; }

        /// <summary>
        /// Factory method that is used to create a new instance of the encapsulated type.
        /// </summary>
        public Delegate Factory { get; }

        /// <summary>
        /// Gets <see cref="ParsingOptionInfo"/> that matches the given token.
        /// </summary>
        /// <param name="token">Token to be matched.</param>
        /// <returns><see cref="ParsingOptionInfo"/> that matches the given token.</returns>
        public ParsingOptionInfo GetMatchingOptionInfo(IOptionToken token)
            => Options.SingleOrDefault(x => x.MatchToken(token));

        /// <summary>
        /// Gets <see cref="ParsingValueInfo"/> at the given position.
        /// </summary>
        /// <param name="index">Index of the <see cref="ParsingValueInfo"/> to be found.</param>
        /// <returns><see cref="ParsingValueInfo"/> at the given position.</returns>
        public ParsingValueInfo GetValueInfoAt(int index)
            => Values.ElementAt(index);
    }
}