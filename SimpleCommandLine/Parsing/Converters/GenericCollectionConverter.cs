using System;
using System.Collections.Generic;

namespace SimpleCommandLine.Parsing.Converters
{
    internal class GenericCollectionConverter : ArrayConverter
    {
        private readonly Type type;

        public GenericCollectionConverter(Type type, IValueConverter valueConverter)
            : base(type.GenericTypeArguments[0], valueConverter)
        {
            this.type = type ?? throw new ArgumentNullException(nameof(type));
        }

        public override ParsingResult Convert(IReadOnlyList<string> values, IFormatProvider formatProvider)
        {
            var result = base.Convert(values, formatProvider);
            if (result.IsError)
                return result;
            return ParsingResult.Success(
                Activator.CreateInstance(type, new object[] { result.AsSuccess.Result }));
        }
    }
}