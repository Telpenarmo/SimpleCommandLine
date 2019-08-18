using System;
using System.Reflection;

namespace SimpleCommandLine.Registration.Validation
{
    internal class PropertyValidator : IPropertyValidator
    {
        public bool Verify(PropertyInfo propertyInfo)
        {
            if (propertyInfo is null)
                throw new ArgumentNullException(nameof(propertyInfo));

            var propertyType = propertyInfo.PropertyType;
            if (!propertyInfo.CanWrite)
                throw new InvalidOperationException("Argument property must have \"set\" accesor.");
            if (propertyType.IsAbstract)
                throw new InvalidOperationException("Type of the property must not be abstract.");
            if (propertyType.GetCollectionElementType()?.IsCollection() ?? false)
                throw new NotSupportedException("Nested collections are not supported.");
            if (propertyType.GetCustomAttribute<FlagsAttribute>() != null)
                throw new NotSupportedException("Flag enumerations are not supported.");
            else
                return true;
        }
    }
}