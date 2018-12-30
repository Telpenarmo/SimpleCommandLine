using System;

namespace SimpleCommandLine.Parsing.Converters
{
    class DoubleValueConverter : IValueConverter<double>
    {
        public double Convert(string str, IFormatProvider formatProvider)
        {
            if (double.TryParse(str, System.Globalization.NumberStyles.Number
                | System.Globalization.NumberStyles.AllowExponent
                | System.Globalization.NumberStyles.AllowCurrencySymbol,
                formatProvider, out double result))
                return result;
            else throw new FormatException("Value must be a number.");
        }

        object IValueConverter.Convert(string str, IFormatProvider formatProvider) => Convert(str, formatProvider);
    }
}
