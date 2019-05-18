using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleCommandLine.Tokenization.Tokens
{
    /// <summary>
    /// Represents a group of option tokens.
    /// </summary>
    public class OptionsGroupToken : IOptionToken
    {
        /// <summary>
        /// Constructs a new instance of <see cref="OptionsGroupToken"/>.
        /// </summary>
        /// <param name="tokens">Option tokens that form a group.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="tokens"/> is null.</exception>
        public OptionsGroupToken(IEnumerable<ShortOptionToken> tokens)
            => Tokens = tokens ?? throw new ArgumentNullException(nameof(tokens));

        /// <summary>
        /// Gets option tokens that form this group.
        /// </summary>
        public IEnumerable<ShortOptionToken> Tokens { get; }

        public bool Equals(IArgumentToken other)
            => other is OptionsGroupToken token ? token.Tokens.SequenceEqual(Tokens) : false;
    }
}