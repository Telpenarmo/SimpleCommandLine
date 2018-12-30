using System;

namespace SimpleCommandLine.Parsing.Converters
{
    class UIntValueConverter : IValueConverter<uint>
    {
        public uint Convert(string str, IFormatProvider formatProvider)
        {
            if (uint.TryParse(str,
                System.Globalization.NumberStyles.Number |
                System.Globalization.NumberStyles.AllowExponent |
                System.Globalization.NumberStyles.AllowCurrencySymbol,
                formatProvider, out uint result))
                return result;
            else throw new FormatException("Value must be a positive integer.");
        }

        object IValueConverter.Convert(string str, IFormatProvider formatProvider) => Convert(str, formatProvider);
    }
}
