using System;

namespace SimpleCommandLine.Parsing.Converters
{
    public class FloatValueConverter : IValueConverter
    {
        public object Convert(string value, IFormatProvider formatProvider)
        {
            if (float.TryParse(value, out float result))
                return result;
            else throw new FormatException($"\"{value}\" is not a valid floating-point number.");
        }
    }
}