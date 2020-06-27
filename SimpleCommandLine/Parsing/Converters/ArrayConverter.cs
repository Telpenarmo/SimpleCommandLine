using System;
using System.Collections.Generic;

namespace SimpleCommandLine.Parsing.Converters
{
    internal class ArrayConverter : IMultipleValueConverter
    {
        private readonly Type elementType;
        private readonly ISingleValueConverter elementConverter;

        public ArrayConverter(Type elementType, ISingleValueConverter elementConverter)
        {
            this.elementType = elementType;
            this.elementConverter = elementConverter;
        }

        public ParsingResult Convert(IReadOnlyList<string> values, IFormatProvider formatProvider)
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