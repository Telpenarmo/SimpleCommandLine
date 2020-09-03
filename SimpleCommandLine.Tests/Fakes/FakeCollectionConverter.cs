using SimpleCommandLine.Parsing;
using SimpleCommandLine.Tests.Fakes;
using System.Collections.Generic;

namespace SimpleCommandLine.Tests.Fakes
{
    class FakeCollectionConverter : IMultipleValueConverter
    {
        public IEnumerable<IConverter> ElementConverters => EnumerableExtensions.Repeat(new FakeConverter());

        public ParsingResult Convert(IReadOnlyList<dynamic> values) => ParsingResult.Success(values);
    }

}