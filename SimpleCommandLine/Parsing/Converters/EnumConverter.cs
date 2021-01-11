using System;
using System.Linq;

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
                if (acceptNumerical)
                    return ParsingResult.Success(Enum.ToObject(type, System.Convert.ChangeType(s, underlyingType)));
                else return ParsingResult.Error("Numerical values are not accepted.");
            }
            if (Enum.TryParse(type, s, ignoreCase, out object result))
                return ParsingResult.Success(result);
            return ParsingResult.Error("Value invalid.");
        }
    }
}