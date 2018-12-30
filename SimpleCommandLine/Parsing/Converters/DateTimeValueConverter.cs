using System;

namespace SimpleCommandLine.Parsing.Converters
{
    class DateTimeValueConverter : IValueConverter<DateTime>
    {
        public DateTime Convert(string str, IFormatProvider formatProvider)
        {
            if (DateTime.TryParse(str, formatProvider,
                System.Globalization.DateTimeStyles.AllowInnerWhite
                | System.Globalization.DateTimeStyles.AllowLeadingWhite
                | System.Globalization.DateTimeStyles.AllowTrailingWhite
                | System.Globalization.DateTimeStyles.AllowWhiteSpaces, out DateTime result))
                return result;
            else throw new FormatException("Value is not valid.");
        }

        object IValueConverter.Convert(string str, IFormatProvider formatProvider) => Convert(str, formatProvider);
    }
}
