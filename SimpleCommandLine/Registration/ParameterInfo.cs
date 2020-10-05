using System;
using System.Reflection;

namespace SimpleCommandLine.Registration
{
    /// <summary>
    /// Encapsulates the <see cref="PropertyInfo"/> representing a command-line argument.
    /// </summary>
    internal sealed class ParameterInfo
    {
        /// <summary>
        /// Creates a new instance of <see cref="ParameterInfo"/>.
        /// </summary>
        /// <param name="propertyInfo">A property to encapsulate.</param>
        public ParameterInfo(Type type, Action<object?, object?> valueSetter, ParameterAttribute? attribute = null)
        {
            SetValue = valueSetter;
            Type = type;

            if (type.IsTuple())
            {
                var num = Type.GetTupleElementTypes().Length;
                Maximum = num;
                Minimum = num;
            }
            else
            {
                Maximum = attribute?.Maximum ?? int.MaxValue;
                Minimum = attribute?.Minimum ?? 0;
            }
        }

        public Type Type { get; }

        /// <summary>
        /// For a collection argument, gets the maximal number of values it may get;
        /// For a tuple it is the number of its values; otherwise ignored.
        /// </summary>
        public int Maximum { get; }

        /// <summary>
        /// For a collection argument, gets the minimal number of values it may get;
        /// For a tuple it is the number of its values; otherwise ignored.
        /// </summary>
        public int Minimum { get; }

        public Action<object?, object?> SetValue { get; }
    }
}