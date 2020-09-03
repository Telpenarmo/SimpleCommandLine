using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleCommandLine.Parsing.Converters
{
    internal class FlagsEnumConverter : IMultipleValueConverter
    {
        public FlagsEnumConverter(Type type, bool ignoreCase, bool acceptNumerical)
        {
            var converter = new EnumConverter(type, ignoreCase, acceptNumerical);
            ElementConverters = EnumerableExtensions.Repeat(converter);
        }

        public IEnumerable<IConverter> ElementConverters { get; }

        public ParsingResult Convert(IReadOnlyList<dynamic> values)
        {
            return ParsingResult.Success(values.Aggregate((a,b) => a | b));
        }
    }
}