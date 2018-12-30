using System;
using SimpleCommandLine.Parsing;

namespace SimpleCommandLine
{
    internal class StringValueConverter : IValueConverter<string>
    {
        public string Convert(string str, IFormatProvider formatProvider) => str;

        object IValueConverter.Convert(string str, IFormatProvider formatProvider) => Convert(str, formatProvider);
    }
}