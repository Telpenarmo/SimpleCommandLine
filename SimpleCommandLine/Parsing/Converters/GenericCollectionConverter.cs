using System;
using System.Collections.Generic;

namespace SimpleCommandLine.Parsing.Converters
{
    internal class GenericCollectionConverter : ArrayConverter
    {
        private readonly Type type;

        public GenericCollectionConverter(Type collectionType, Type elementType, IValueConverter valueConverter) : base(elementType, valueConverter)
        {
            type = collectionType.MakeGenericType(elementType) ?? throw new ArgumentNullException(nameof(collectionType));
        }

        public GenericCollectionConverter(Type type, IValueConverter valueConverter) : base(type.GetGenericArguments()[0], valueConverter)
        {
            this.type = type ?? throw new ArgumentNullException(nameof(type));
        }

        public override object Convert(IReadOnlyList<string> values, IFormatProvider formatProvider)
        {
            var array = base.Convert(values, formatProvider);
            return Activator.CreateInstance(type, new object[] { array });
        }
    }
}