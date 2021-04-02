using System;
using System.Linq;
using Converter = System.Convert;
using static SimpleCommandLine.Parsing.ParsingResult;

namespace SimpleCommandLine.Parsing.Converters
{
    internal class EnumConverter : ISingleValueConverter
    {
        private readonly Type type;
        private readonly Type underlyingType;
        private readonly bool ignoreCase;
        private readonly bool acceptNumerical;

        public EnumConverter(Type type, bool ignoreCase, bool acceptNumerical)
        {
            this.type = type.IsEnum ? type
                : throw new ArgumentException("Type given to EnumConverter must be an enum.");
            underlyingType = Enum.GetUnderlyingType(type);
            this.ignoreCase = ignoreCase;
            this.acceptNumerical = acceptNumerical;
        }

        public ParsingResult Convert(string value, IFormatProvider formatProvider)
        {
            string s = ignoreCase ? value.ToLower() : value;
            if (value.All(c => char.IsDigit(c)))
            {
                if (!acceptNumerical)
                    return Error("Numerical values are not accepted.");

                var num = Converter.ChangeType(s, underlyingType);
                if (Enum.IsDefined(type, num))
                    return Success(Enum.ToObject(type, num));
                return Error($"Value {num} is not defined in {type} enumeration.");
            }
            if (Enum.TryParse(type, s, ignoreCase, out object result))
                return Success(result);
            return Error("Value invalid.");
        }
    }
}