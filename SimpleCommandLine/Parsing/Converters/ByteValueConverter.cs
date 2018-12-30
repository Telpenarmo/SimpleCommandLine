using System;

namespace SimpleCommandLine.Parsing.Converters
{
    class ByteValueConverter : IValueConverter<byte>
    {
        public byte Convert(string str, IFormatProvider formatProvider)
        {
            if (byte.TryParse(str, System.Globalization.NumberStyles.AllowLeadingWhite | System.Globalization.NumberStyles.AllowTrailingWhite, formatProvider, out byte result))
                return result;
            else throw new FormatException("Value must be a positive integer less than 256.");
        }

        object IValueConverter.Convert(string str, IFormatProvider formatProvider) => Convert(str, formatProvider);
    }
}
