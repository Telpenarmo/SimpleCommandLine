using System;

namespace SimpleCommandLine.Parsing.Converters
{
    public class IntValueConverter : IValueConverter
    {
        public bool Convert(string value, IFormatProvider formatProvider, out object result)
        {
            result = null;
            if (!int.TryParse(value, out int i)) return false;
            result = i;
            return true;
        }
    }
}