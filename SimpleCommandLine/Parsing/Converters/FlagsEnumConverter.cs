using System;
using System.Collections.Generic;
using System.Linq;
using Converter = System.Convert;

namespace SimpleCommandLine.Parsing.Converters
{
    internal class FlagsEnumConverter : IMultipleValueConverter
    {
        public FlagsEnumConverter(Type type, bool ignoreCase, bool acceptNumerical)
        {
            var converter = new EnumConverter(type, ignoreCase, acceptNumerical);
            ElementConverters = EnumerableExtensions.Repeat(converter);
            targetType = type;
        }

        public IEnumerable<IConverter> ElementConverters { get; }
        private readonly Type targetType;

        public ParsingResult Convert(IReadOnlyList<dynamic> values)
        {
            var num = values.Select(ConvertValue).Aggregate((a, b) => a | b);
            return ParsingResult.Success(Enum.ToObject(targetType, num));
        }

        private static ulong ConvertValue(dynamic value)
        {
            Type currentType = value.GetType();
            if (currentType.IsInteger())
            {
                if ((long)value < 0)
                    throw new ArgumentException($"{value.ToString()} cannot be converted to enum.");
                return Converter.ToUInt64(value);
            }
            if (currentType.IsEnum)
                return Converter.ToUInt64(value);
            else throw new ArgumentException($"{value.ToString()} cannot be converted to enum.");
        }
    }
}