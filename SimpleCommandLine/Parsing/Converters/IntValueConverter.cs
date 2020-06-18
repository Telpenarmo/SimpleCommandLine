using System;

namespace SimpleCommandLine.Parsing.Converters
{
    public class IntValueConverter : IValueConverter
    {
        public ParsingResult Convert(string value, IFormatProvider formatProvider)
        {
            if (int.TryParse(value, out int i))
                return ParsingResult.Success(i);
            return ParsingResult.Error($"{value} is not a valid integer number.");
        }
    }
}