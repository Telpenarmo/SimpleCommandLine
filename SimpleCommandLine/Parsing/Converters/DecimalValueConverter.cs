using System;

namespace SimpleCommandLine.Parsing.Converters
{
    class DecimalValueConverter : IValueConverter<decimal>
    {
        public decimal Convert(string str, IFormatProvider formatProvider)
        {
            if (decimal.TryParse(str, System.Globalization.NumberStyles.Number
                | System.Globalization.NumberStyles.AllowExponent
                | System.Globalization.NumberStyles.AllowCurrencySymbol,
                formatProvider, out decimal result))
                return result;
            else throw new FormatException("Value must be a number.");
        }

        object IValueConverter.Convert(string str, IFormatProvider formatProvider) => Convert(str, formatProvider);
    }
}
