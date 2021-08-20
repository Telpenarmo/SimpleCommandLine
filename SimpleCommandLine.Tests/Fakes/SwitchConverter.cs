using System;
using SimpleCommandLine.Parsing;

namespace SimpleCommandLine.Tests.Fakes
{
    class SwitchConverter : ISingleValueConverter
    {
        public ParsingResult Convert(string value, IFormatProvider formatProvider)
            => ParsingResult.Success(value);

        public object DefaultValue => new();
    }
}