using System;

namespace SimpleCommandLine.Parsing.Converters
{
    public class StringValueConverter : IValueConverter
    {
        public ParsingResult Convert(string value, IFormatProvider formatProvider)
            => ParsingResult.Success(value);
    }
}