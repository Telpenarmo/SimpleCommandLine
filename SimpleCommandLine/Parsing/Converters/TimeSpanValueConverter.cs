using System;

namespace SimpleCommandLine.Parsing.Converters
{
    internal class TimeSpanValueConverter : IValueConverter<TimeSpan>
    {
        public TimeSpan Convert(string str, IFormatProvider formatProvider)
        {
            if (TimeSpan.TryParse(str, formatProvider, out TimeSpan result))
                return result;
            else throw new FormatException("Value is not valid.");
        }

        object IValueConverter.Convert(string str, IFormatProvider formatProvider) => Convert(str, formatProvider);
    }
}
