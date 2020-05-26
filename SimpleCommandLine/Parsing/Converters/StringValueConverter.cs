using System;

namespace SimpleCommandLine.Parsing.Converters
{
    public class StringValueConverter : IValueConverter
    {
        public object Convert(string value, IFormatProvider formatProvider)
            => value;
    }
}