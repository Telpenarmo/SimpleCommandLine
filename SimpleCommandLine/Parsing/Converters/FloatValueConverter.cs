using System;

namespace SimpleCommandLine.Parsing.Converters
{
    class FloatValueConverter : IValueConverter<float>
    {
        public float Convert(string str, IFormatProvider formatProvider)
        {
            if (float.TryParse(str, System.Globalization.NumberStyles.Number
                | System.Globalization.NumberStyles.AllowExponent
                | System.Globalization.NumberStyles.AllowCurrencySymbol,
                formatProvider, out float result))
                return result;
            else throw new FormatException("Value must be a number.");
        }

        object IValueConverter.Convert(string str, IFormatProvider formatProvider) => Convert(str, formatProvider);
    }
}
