﻿using System;
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
        /// Determines whether this argument may get multiple arguments.
        /// </summary>
        public bool IsCollection
            => PropertyType != typeof(string) && (PropertyType.IsArray || typeof(System.Collections.IEnumerable).IsAssignableFrom(PropertyType));

        /// <summary>
        /// If this argument is a collection, gets or sets the minimal number of values it may get; otherwise ignored.
        /// </summary>
        public int Minimum { get; set; } = 1;

        /// <summary>
        /// If this argument is a collection, gets or sets the maximal number of values it may get; otherwise ignored.
        /// </summary>
        public int? Maximum { get; set; } = null;

        /// <summary>
        /// Sets the property value for a specified object.
        /// </summary>
        /// <param name="obj">The object whose property value will be set.</param>
        /// <param name="value">The new property value.</param>
        public void SetValue(object obj, object value) => propertyInfo.SetValue(obj, value);
    }
}