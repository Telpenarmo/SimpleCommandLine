using System;
using System.Collections.Generic;

namespace SimpleCommandLine.Parsing.Converters
{
    internal class GenericCollectionConverter : IMultipleValueConverter
    {
        private readonly Type type;
        private readonly ArrayConverter arrayConverter;

        public GenericCollectionConverter(Type type, Type elementType, IConverter elementConverter)
        {
            arrayConverter = new ArrayConverter(elementType, elementConverter);
            this.type = type;
        }

        public IEnumerable<IConverter> ElementConverters => arrayConverter.ElementConverters;

        public ParsingResult Convert(IReadOnlyList<dynamic> values)
        {
            var result = arrayConverter.Convert(values);
            if (result.IsError) return result;
            return ParsingResult.Success(Activator.CreateInstance(type, result.ResultObject));
        }
    }
}