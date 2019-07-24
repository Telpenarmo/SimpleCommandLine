using System;
using System.Collections.Generic;

namespace SimpleCommandLine.Parsing.Converters
{
    internal class ArrayConverter : CollectionConverter
    {
        public ArrayConverter(Type elementType, IValueConverter valueConverter) : base(elementType, valueConverter) { }

        public override object Convert(IReadOnlyList<string> values, IFormatProvider formatProvider)
        {
            var array = Array.CreateInstance(elementType, values.Count);
            for (var i = 0; i < array.Length; i++)
                array.SetValue(valueConverter.Convert(values[i], formatProvider), i);
            return array;
        }
    }
}