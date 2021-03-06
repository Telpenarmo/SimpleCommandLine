﻿using System;
using System.Reflection;

namespace SimpleCommandLine.Registration
{
    /// <summary>
    /// Encapsulates a <see cref="PropertyInfo"/> representing a command-line value.
    /// </summary>
    internal class ParsingValueInfo : ParsingArgumentInfo
    {
        /// <summary>
        /// Creates a new instance of <see cref="ParsingValueInfo"/>.
        /// </summary>
        /// <param name="propertyInfo">A property to encapsulate.</param>
        public ParsingValueInfo(PropertyInfo propertyInfo) : base(propertyInfo)
        {
            attribute = propertyInfo.GetCustomAttribute<ValueAttribute>()
                ?? throw new ArgumentNullException(nameof(propertyInfo), "Given property doesn't have the required attribute.");
        }

        /// <summary>
        /// Creates a new instance of <see cref="ParsingValueInfo"/>.
        /// </summary>
        /// <param name="propertyInfo">A property to encapsulate.</param>
        /// <param name="attribute">Attribute containing configuration of the value.</param>
        public ParsingValueInfo(PropertyInfo propertyInfo, ValueAttribute attribute) : this(propertyInfo)
            => this.attribute = attribute ?? throw new ArgumentNullException(nameof(attribute));

        private ValueAttribute Attribute => attribute as ValueAttribute;

        /// <summary>
        /// Gets the place of this value in the order of values.
        /// </summary>
        public uint Index => Attribute.Index;
    }
}