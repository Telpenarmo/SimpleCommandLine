using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleCommandLine.Tokens
{
    /// <summary>
    /// Represents a group of command-line values.
    /// <seealso cref="ValueToken"/>
    /// </summary>
    public class ValuesGroupToken : ValueToken
    {
        /// <summary>
        /// Creates a new onstance of <see cref="ValuesGroupToken"/> type.
        /// </summary>
        /// <param name="values">Tokens that form the group.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="values"/> is null.</exception>
        public ValuesGroupToken(IEnumerable<ValueToken> values, string original) : base(original)
            => Tokens = values?.ToList();

        /// <summary>
        /// Gets all elements of this token.
        /// </summary>
        public IReadOnlyList<ValueToken> Tokens { get; }

        public override bool Equals(IArgumentToken other)
            => other is ValuesGroupToken collectionToken && Tokens.SequenceEqual(collectionToken.Tokens);
    }
}