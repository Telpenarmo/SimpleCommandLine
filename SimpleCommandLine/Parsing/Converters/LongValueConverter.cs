using System;

namespace SimpleCommandLine.Parsing.Converters
{
    class LongValueConverter : IValueConverter<long>
    {
        public long Convert(string str, IFormatProvider formatProvider)
        {
            if (long.TryParse(str,
                System.Globalization.NumberStyles.Number |
                System.Globalization.NumberStyles.AllowExponent |
                System.Globalization.NumberStyles.AllowCurrencySymbol,
                formatProvider, out long result))
                return result;
            else throw new FormatException("Value must be an integer.");
        }

        object IValueConverter.Convert(string str, IFormatProvider formatProvider) => Convert(str, formatProvider);
    }
}
