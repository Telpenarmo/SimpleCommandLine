using System;

namespace SimpleCommandLine.Parsing.Converters
{
    class GuidValueConverter : IValueConverter<Guid>
    {
        public Guid Convert(string str, IFormatProvider formatProvider)
        {
            if (Guid.TryParse(str, out Guid result))
                return result;
            else throw new FormatException("Value must be 32 digits.");
        }

        object IValueConverter.Convert(string str, IFormatProvider formatProvider) => Convert(str, formatProvider);
    }
}
