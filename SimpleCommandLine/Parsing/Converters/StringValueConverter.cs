using System;

namespace SimpleCommandLine.Parsing.Converters
{
    public class StringValueConverter : IValueConverter
    {
        public bool Convert(string value, IFormatProvider formatProvider, out object result)
        {
            result = value;
            return true;
        }
    }
}