﻿using System;
using System.Reflection;
using SimpleCommandLine.Tokenization.Tokens;

namespace SimpleCommandLine.Registration
{
    /// <summary>
    /// Encapsulates a <see cref="PropertyInfo"/> representing a command-line option.
    /// </summary>
    internal class ParsingOptionInfo : ParsingArgumentInfo
    {
        /// <summary>
        /// Creates a new instance of <see cref="ParsingOptionInfo"/> class.
        /// </summary>
        /// <param name="propertyInfo">A property to encapsulate.</param>
        public ParsingOptionInfo(PropertyInfo propertyInfo) : base(propertyInfo)
        {
            attribute = propertyInfo.GetCustomAttribute<OptionAttribute>()
                    ?? throw new ArgumentNullException(nameof(propertyInfo), "Given property doesn't have the required attribute.");
        }

        /// <summary>
        /// Creates a new instance of <see cref="ParsingOptionInfo"/> class.
        /// </summary>
        /// <param name="propertyInfo">A property to encapsulate.</param>
        /// <param name="attribute">Attribute containing configuration of the option.</param>
        public ParsingOptionInfo(PropertyInfo propertyInfo, OptionAttribute attribute) : base(propertyInfo)
        {
            this.attribute = attribute ?? throw new ArgumentNullException(nameof(attribute));
        }

        private readonly OptionAttribute attribute;

        /// <summary>
        /// Checks if given token matches current option.
        /// </summary>
        /// <param name="token">The token to match.</param>
        /// <returns>True, if given token matches this option; false otherwise.</returns>
        public bool MatchToken(IOptionToken token)
        {
            switch (token)
            {
                case ShortOptionToken sToken:
                    return MatchToken(sToken);
                case LongOptionToken lToken:
                    return MatchToken(lToken);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Checks if given token matches current option.
        /// </summary>
        /// <param name="token">The token to match.</param>
        /// <returns>True, if given token matches this option; false otherwise.</returns>
        public bool MatchToken(ShortOptionToken token) => token.Value == attribute.ShortName;

        /// <summary>
        /// Checks if given token matches current option.
        /// </summary>
        /// <param name="token">The token to match.</param>
        /// <returns>True, if given token matches this option; false otherwise.</returns>
        public bool MatchToken(LongOptionToken token) => token.Value == attribute?.LongName;
    }
}
