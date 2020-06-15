using System;

namespace SimpleCommandLine.Parsing.Converters
{
    public class FloatValueConverter : IValueConverter
    {
        public bool Convert(string value, IFormatProvider formatProvider, out object result)
        {
            result = null;
            if (!float.TryParse(value, out float f)) return false;
            result = f;
            return true;
        }
    }
}