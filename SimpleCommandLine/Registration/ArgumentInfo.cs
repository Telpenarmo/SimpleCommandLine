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
        protected PropertyInfo propertyInfo;
        protected ArgumentAttribute attribute;

        /// <summary>
        /// Creates a new instance of <see cref="ArgumentInfo"/>.
        /// </summary>
        /// <param name="propertyInfo">A property to encapsulate.</param>
        protected ArgumentInfo(PropertyInfo propertyInfo)
            => this.propertyInfo = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));

        protected Type PropertyType => propertyInfo.PropertyType;

        /// <summary>
        /// Determines whether this argument may get multiple arguments.
        /// </summary>
        public bool IsCollection => PropertyType.IsCollection();
        public int Minimum => attribute.Minimum;
        public int Maximum => attribute.Maximum;

        /// <summary>
        /// Sets the property value for a specified object.
        /// </summary>
        /// <param name="obj">The object whose property value will be set.</param>
        /// <param name="value">The new property value.</param>
        public void SetValue(object obj, object value) => propertyInfo.SetValue(obj, value);

        /// <summary>
        /// Gets converter that operates the type of this property.
        /// </summary>
        /// <param name="convertersFactory">Used to get the applicable converter.</param>
        /// <returns>Object converting string to this property's type; null if no suitable converter was registered.</returns>
        public IConverter ChooseConverter(ConvertersFactory convertersFactory)
            => convertersFactory.GetConverter(PropertyType);
    }
}