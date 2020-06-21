using System;
using System.Collections.Generic;

namespace SimpleCommandLine.Parsing.Converters
{
    internal class ArrayConverter : CollectionConverter
    {
        public ArrayConverter(Type elementType, IValueConverter valueConverter)
            : base(elementType, valueConverter) { }

        public override ParsingResult Convert(IReadOnlyList<string> values, IFormatProvider formatProvider)
        {
            var array = Array.CreateInstance(elementType, values.Count);
            for (var i = 0; i < array.Length; i++){
                var res = elementConverter.Convert(values[i], formatProvider);
                if (res.IsError) return res;
                array.SetValue(res.ResultObject, i);
            }
            return ParsingResult.Success(array);
        }
    }
}