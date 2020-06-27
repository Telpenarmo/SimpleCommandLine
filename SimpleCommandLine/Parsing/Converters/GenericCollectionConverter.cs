using System;
using System.Collections.Generic;

namespace SimpleCommandLine.Parsing.Converters
{
    internal class GenericCollectionConverter : IMultipleValueConverter
    {
        private readonly Type type;
        private readonly ArrayConverter arrayConverter;

        public GenericCollectionConverter(Type type, ISingleValueConverter elementConverter)
        {
            arrayConverter = new ArrayConverter(type.GenericTypeArguments[0], elementConverter);
            this.type = type;
        }

        public ParsingResult Convert(IReadOnlyList<string> values, IFormatProvider formatProvider)
        {
            var result = arrayConverter.Convert(values, formatProvider);
            if (result.IsError) return result;
            return ParsingResult.Success(
                Activator.CreateInstance(type, new object[] { result.ResultObject }));
        }
    }
}