using System;
using System.Collections.Generic;

namespace SimpleCommandLine.Parsing.Converters
{
    internal class ArrayConverter : CollectionConverter
    {
        public ArrayConverter(Type elementType, IValueConverter valueConverter) : base(elementType, valueConverter) { }

        public override bool Convert(IReadOnlyList<string> values, IFormatProvider formatProvider, out object result)
        {
            var array = Array.CreateInstance(elementType, values.Count);
            for (var i = 0; i < array.Length; i++){
                valueConverter.Convert(values[i], formatProvider, out object o);
                array.SetValue(o, i);
            }
            result = array;
            return true;
        }
    }
}