using System;
using System.Collections.Generic;
using System.Linq;
using SimpleCommandLine.Tokens;

namespace SimpleCommandLine.Registration
{
    /// <summary>
    /// Encapsulates the type.
    /// </summary>
    internal class TypeInfo
    {
        /// <summary>
        /// Creates a new instance od <see cref="TypeInfo"/> class.
        /// </summary>
        /// <param name="values">Collection of properties representing command-line values.</param>
        /// <param name="options">Collection of properties representing command-line options.</param>
        /// <param name="factory">Factory method that is used to create a new instance of the encapsulated type.</param>
        public TypeInfo(IEnumerable<ValueInfo> values, IEnumerable<OptionInfo> options, Delegate factory)
        {
            Values = values.OrderBy(x => x.Index).ToArray();
            Options = options.ToArray();
            Factory = factory;
            Aliases = Enumerable.Empty<string>();
        }

        public TypeInfo(IEnumerable<ValueInfo> values, IEnumerable<OptionInfo> options, Delegate factory, IEnumerable<string> aliases)
        {
            Values = values.OrderBy(x => x.Index).ToArray();
            Options = options.ToArray();
            Factory = factory;
            Aliases = aliases;
        }

        public IEnumerable<string> Aliases { get; }
        public IReadOnlyList<ValueInfo> Values { get; }
        public IReadOnlyList<OptionInfo> Options { get; }

        /// <summary>
        /// Factory method that is used to create a new instance of the encapsulated type.
        /// </summary>
        public Delegate Factory { get; }

        /// <summary>
        /// Gets <see cref="OptionInfo"/> that matches the given token.
        /// </summary>
        /// <param name="token">Token to be matched.</param>
        /// <returns><see cref="OptionInfo"/> that matches the given token.</returns>
        public OptionInfo GetMatchingOptionInfo(OptionToken token)
            => Options.SingleOrDefault(x => x.MatchToken(token));
    }
}