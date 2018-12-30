using System;

namespace SimpleCommandLine.Parsing.Converters
{
    class ShortValueConverter : IValueConverter<short>
    {
        public short Convert(string str, IFormatProvider formatProvider)
        {
            if (short.TryParse(str,
                System.Globalization.NumberStyles.Number |
                System.Globalization.NumberStyles.AllowCurrencySymbol,
                formatProvider, out short result))
                return result;
            else throw new FormatException("Value must be an integer.");
        }

        object IValueConverter.Convert(string str, IFormatProvider formatProvider) => Convert(str, formatProvider);
    }
}
