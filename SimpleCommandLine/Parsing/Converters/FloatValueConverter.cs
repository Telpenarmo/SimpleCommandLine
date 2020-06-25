using System;

namespace SimpleCommandLine.Parsing.Converters
{
    public class FloatValueConverter : IValueConverter
    {
        public ParsingResult Convert(string value, IFormatProvider formatProvider)
        {
            if (float.TryParse(value, out float f))
                return ParsingResult.Success(f);
            return ParsingResult.Error($"\"{value}\" is not a valid floating-point number.");
        }
    }
}