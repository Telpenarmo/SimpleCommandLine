using System;
using System.Reflection;

namespace SimpleCommandLine.Registration
{
    /// <summary>
    /// Encapsulates the <see cref="PropertyInfo"/> representing a command-line argument.
    /// </summary>
    internal abstract class ParsingArgumentInfo
    {
        protected PropertyInfo propertyInfo;

        /// <summary>
        /// Creates a new instance of <see cref="ParsingArgumentInfo"/>.
        /// </summary>
        /// <param name="propertyInfo">A property to encapsulate.</param>
        protected ParsingArgumentInfo(PropertyInfo propertyInfo)
            => this.propertyInfo = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));

        /// <summary>
        /// This property's <see cref="Type"/>.
        /// </summary>
        public Type PropertyType => propertyInfo.PropertyType;

        /// <summary>
        /// Sets the property value for a specified object.
        /// </summary>
        /// <param name="obj">The object whose property value will be set.</param>
        /// <param name="value">The new property value.</param>
        public void SetValue(object obj, object value) => propertyInfo.SetValue(obj, value);
    }
}