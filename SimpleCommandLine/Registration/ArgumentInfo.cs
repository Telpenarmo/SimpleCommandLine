using System;
using System.Reflection;
using SimpleCommandLine.Parsing;

namespace SimpleCommandLine.Registration
{
    /// <summary>
    /// Encapsulates the <see cref="PropertyInfo"/> representing a command-line argument.
    /// </summary>
    internal abstract class ArgumentInfo
    {
        protected ArgumentAttribute attribute;

        /// <summary>
        /// Creates a new instance of <see cref="ArgumentInfo"/>.
        /// </summary>
        /// <param name="propertyInfo">A property to encapsulate.</param>
        protected ArgumentInfo(Type type, Action<object, object> valueSetter, ArgumentAttribute attribute)
        {
            this.attribute = attribute;
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
                Maximum = attribute.Maximum;
                Minimum = attribute.Minimum;
            }
        }

        protected Type Type { get; }

        /// <summary>
        /// Determines whether this argument may get multiple arguments.
        /// </summary>
        public bool IsMulltiValued => Type.IsCollection() || Type.IsTuple();

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

        public Action<object, object> SetValue { get; }

        /// <summary>
        /// Gets converter that operates the type of this property.
        /// </summary>
        /// <param name="convertersFactory">Used to get the applicable converter.</param>
        /// <returns>Object converting string to this property's type; null if no suitable converter was registered.</returns>
        public IConverter ChooseConverter(ConvertersFactory convertersFactory)
            => convertersFactory.GetConverter(Type);
    }
}