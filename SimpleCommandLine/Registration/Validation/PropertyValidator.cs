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
            else if (propertyType.IsAbstract)
                throw new InvalidOperationException("Type of the property must not be abstract.");
            else if (propertyType != typeof(string) && (propertyType.IsArray || typeof(System.Collections.IEnumerable).IsAssignableFrom(propertyType)))
                throw new InvalidOperationException("Collections are not yet supported.");
            else
                return true;
        }
    }
}