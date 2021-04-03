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
            {
                if (!elementType.IsAssignableFrom(values[i].GetType()))
                    throw new ArgumentException($"Value of index {i} is not assignable" +
                        $" to the expected type {elementType.ToString()}.");
                array.SetValue(values[i], i);
            }
            return ParsingResult.Success(array);
        }
    }
}