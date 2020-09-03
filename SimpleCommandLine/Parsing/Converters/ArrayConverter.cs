using System;
using System.Collections.Generic;

namespace SimpleCommandLine.Parsing.Converters
{
    internal class ArrayConverter : IMultipleValueConverter
    {
        private readonly Type elementType;

        public ArrayConverter(Type elementType, IConverter elementConverter)
        {
            this.elementType = elementType;
            ElementConverters = EnumerableExtensions.Repeat(elementConverter);
        }

        public IEnumerable<IConverter> ElementConverters { get; }

        public ParsingResult Convert(IReadOnlyList<dynamic> values)
        {
            var array = Array.CreateInstance(elementType, values.Count);
            for (var i = 0; i < array.Length; i++)
                array.SetValue(values[i], i);
            return ParsingResult.Success(array);
        }
    }
}