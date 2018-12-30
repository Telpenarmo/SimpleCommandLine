using System;
using System.Net;

namespace SimpleCommandLine.Parsing.Converters
{
    class IPAddressValueConverter : IValueConverter<IPAddress>
    {
        public IPAddress Convert(string str, IFormatProvider formatProvider)
        {
            if (IPAddress.TryParse(str, out IPAddress result))
                return result;
            else throw new FormatException("Value is not valid.");
        }

        object IValueConverter.Convert(string str, IFormatProvider formatProvider) => Convert(str, formatProvider);
    }
}
