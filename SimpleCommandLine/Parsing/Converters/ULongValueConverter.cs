using System;

namespace SimpleCommandLine.Parsing.Converters
{
    class ULongValueConverter : IValueConverter<ulong>
    {
        public ulong Convert(string str, IFormatProvider formatProvider)
        {
            if (ulong.TryParse(str,
                System.Globalization.NumberStyles.Number |
                System.Globalization.NumberStyles.AllowExponent |
                System.Globalization.NumberStyles.AllowCurrencySymbol,
                formatProvider, out ulong result))
                return result;
            else throw new FormatException("Value must be a positive integer.");
        }

        object IValueConverter.Convert(string str, IFormatProvider formatProvider) => Convert(str, formatProvider);
    }
}
