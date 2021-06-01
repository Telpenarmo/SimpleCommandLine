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
        public TypeInfo(IReadOnlyList<ParameterInfo> values, IReadOnlyDictionary<string, ParameterInfo> options,
            Func<object?> factory) : this(values, options, factory, Enumerable.Empty<string>())
        {
        }

        public TypeInfo(IReadOnlyList<ParameterInfo> values, IReadOnlyDictionary<string, ParameterInfo> options,
            Func<object?> factory, IEnumerable<string> aliases)
        {
            Values = values;
            Options = options;
            Factory = factory;
            Aliases = aliases;
        }

        public IEnumerable<string> Aliases { get; }
        public IReadOnlyList<ParameterInfo> Values { get; }
        public IReadOnlyDictionary<string, ParameterInfo> Options { get; }

        /// <summary>
        /// Factory method that is used to create a new instance of the encapsulated type.
        /// </summary>
        public Func<object?> Factory { get; }

        /// <summary>
        /// Gets <see cref="OptionInfo"/> that matches the given token.
        /// </summary>
        /// <param name="token">Token to be matched.</param>
        /// <returns><see cref="OptionInfo"/> that matches the given token.</returns>
        public ParameterInfo? GetMatchingOptionInfo(OptionToken token)
            => Options.TryGetValue(token.Value, out var result) ? result : null;
    }
}