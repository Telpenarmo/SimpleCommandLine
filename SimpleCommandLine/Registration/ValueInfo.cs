using System;
using System.Reflection;

namespace SimpleCommandLine.Registration
{
    /// <summary>
    /// Encapsulates a <see cref="PropertyInfo"/> representing a command-line value.
    /// </summary>
    internal class ValueInfo : ArgumentInfo
    {
        /// <summary>
        /// Creates a new instance of <see cref="ValueInfo"/>.
        /// </summary>
        /// <param name="propertyInfo">A property to encapsulate.</param>
        /// <param name="attribute">Attribute containing configuration of the value.</param>
        public ValueInfo(Type type, Action<object, object> valueSetter, ValueAttribute attribute)
            : base(type, valueSetter, attribute) { }

        private ValueAttribute Attribute => attribute as ValueAttribute;

        /// <summary>
        /// Gets the place of this value in the order of values.
        /// </summary>
        public uint Index => Attribute.Index;
    }
}