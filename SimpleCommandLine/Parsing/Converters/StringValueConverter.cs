using System;

namespace SimpleCommandLine.Parsing.Converters
{
    public class StringValueConverter : IValueConverter
    {
        public bool Convert(string value, IFormatProvider formatProvider, out object result)
        {
            if (string.IsNullOrEmpty(value))
            {
                result = null;
                return false;
            }
            result = value;
            return true;
        }
    }
}