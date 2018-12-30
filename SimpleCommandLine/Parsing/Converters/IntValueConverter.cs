using System;

namespace SimpleCommandLine.Parsing.Converters
{
    class IntValueConverter : IValueConverter<int>
    {
        public int Convert(string str, IFormatProvider formatProvider)
        {
            if (int.TryParse(str,
                System.Globalization.NumberStyles.Number |
                System.Globalization.NumberStyles.AllowExponent |
                System.Globalization.NumberStyles.AllowCurrencySymbol,
                formatProvider, out int result))
                return result;
            else throw new FormatException("Value must be an integer.");
        }

        object IValueConverter.Convert(string str, IFormatProvider formatProvider) => Convert(str, formatProvider);
    }
}
