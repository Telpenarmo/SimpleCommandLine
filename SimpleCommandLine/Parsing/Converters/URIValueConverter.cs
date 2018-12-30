using System;

namespace SimpleCommandLine.Parsing.Converters
{
    class URIValueConverter : IValueConverter<Uri>
    {
        public Uri Convert(string str, IFormatProvider formatProvider)
        {
            return new Uri(str, UriKind.RelativeOrAbsolute);
        }

        object IValueConverter.Convert(string str, IFormatProvider formatProvider) => Convert(str, formatProvider);
    }
}
