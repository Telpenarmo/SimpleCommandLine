using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleCommandLine.Tokenization.Tokens
{
    /// <summary>
    /// Represents a group of command-line values.
    /// <seealso cref="ValueToken"/>
    /// </summary>
    public class ValuesGroupToken : IValueToken
    {
        /// <summary>
        /// Creates a new onstance of <see cref="ValuesGroupToken"/> type.
        /// </summary>
        /// <param name="values">Tokens that form the group.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="values"/> is null.</exception>
        public ValuesGroupToken(IEnumerable<IValueToken> values)
        {
            Tokens = values?.ToList() ?? throw new ArgumentNullException(nameof(values));
        }

        /// <summary>
        /// Gets all elements of this token.
        /// </summary>
        public IReadOnlyList<IValueToken> Tokens { get; }

        public bool Equals(IArgumentToken other)
            => other is ValuesGroupToken collectionToken ? Tokens.SequenceEqual(collectionToken.Tokens) : false;
    }
}
