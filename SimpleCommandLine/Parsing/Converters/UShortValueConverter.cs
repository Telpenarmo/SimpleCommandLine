using System;

namespace SimpleCommandLine.Parsing.Converters
{
    class UShortValueConverter : IValueConverter<ushort>
    {
        public ushort Convert(string str, IFormatProvider formatProvider)
        {
            if (ushort.TryParse(str,
                System.Globalization.NumberStyles.Number |
                System.Globalization.NumberStyles.AllowExponent |
                System.Globalization.NumberStyles.AllowCurrencySymbol,
                formatProvider, out ushort result))
                return result;
            else throw new FormatException("Value must be a positive integer.");
        }

        object IValueConverter.Convert(string str, IFormatProvider formatProvider) => Convert(str, formatProvider);
    }
}
