using System;
using SimpleCommandLine.Parsing;

namespace SimpleCommandLine.Tests.Fakes
{
    class FakeImplicitConverter : ISingleValueConverter
    {
        public ParsingResult Convert(string value, IFormatProvider formatProvider)
            => ParsingResult.Success(System.Convert.ToBoolean(value));

        public object DefaultValue => true; 
    }
}