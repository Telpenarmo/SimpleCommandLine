using System;

namespace SimpleCommandLine.Parsing.Converters
{
    public class IntValueConverter : IValueConverter
    {
        public object Convert(string value, IFormatProvider formatProvider)
        {
            if (int.TryParse(value, out int result))
                return result;
            else throw new FormatException($"\"{value}\" is not a valid integer number.");
        }
    }
}