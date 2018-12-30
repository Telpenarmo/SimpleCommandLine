using System;

namespace SimpleCommandLine.Parsing.Converters
{
    class SByteValueConverter : IValueConverter<sbyte>
    {
        public sbyte Convert(string str, IFormatProvider formatProvider)
        {
            if (sbyte.TryParse(str, System.Globalization.NumberStyles.Number, formatProvider, out sbyte result))
                return result;
            else throw new FormatException("Value must be a integer between -128 and 127.");
        }

        object IValueConverter.Convert(string str, IFormatProvider formatProvider) => Convert(str, formatProvider);
    }
}
