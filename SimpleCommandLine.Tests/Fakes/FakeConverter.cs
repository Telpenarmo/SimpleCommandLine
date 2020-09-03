using System;
using SimpleCommandLine.Parsing;

namespace SimpleCommandLine.Tests.Fakes
{
    class FakeConverter : ISingleValueConverter
    {
        public ParsingResult Convert(string value, IFormatProvider formatProvider)
            => ParsingResult.Success(value);
    }
}