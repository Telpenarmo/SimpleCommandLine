using System.Collections.Generic;
using Xunit;
using SimpleCommandLine.Parsing;
using SimpleCommandLine.Tests.Fakes;
using SimpleCommandLine.Tokenization.Tokens;

namespace SimpleCommandLine.Tests.Parsing
{
    public class TypeParserTests
    {
        private readonly TokensParser typeParser;
        private FakeObjectBuilder objectBuilder = new FakeObjectBuilder();
        private IEnumerable<IArgumentToken> GetTokens(params IArgumentToken[] arguments) => arguments;

        public TypeParserTests()
        {
            typeParser = new TokensParser(objectBuilder);
        }

        [Fact]
        public void Given_two_options_calls_HandleImplicitOption_twice()
        {
            typeParser.Parse(GetTokens(new ShortOptionToken('o'), new LongOptionToken("option")));
            Assert.Equal(2, objectBuilder.HandleImplicitOptionCallsNumber);
            objectBuilder.Reset();
        }

        [Fact]
        public void Given_option_and_value_calls_and_SetBoundValue()
        {
            IOptionToken optionToken = new ShortOptionToken('o');
            typeParser.Parse(GetTokens(optionToken, new ValueToken("value")));
            Assert.Equal(1, objectBuilder.SetBoundValueCallsNumber);
            Assert.Equal(optionToken, objectBuilder.SetBoundValueArgument(0).Item1);
            Assert.Equal("value", objectBuilder.SetBoundValueArgument(0).Item2);
            objectBuilder.Reset();
        }

        [Fact]
        public void Given_value_calls_SetUnboundValue()
        {
            typeParser.Parse(GetTokens(new ValueToken("value")));
            Assert.Equal(1, objectBuilder.SetUnboundValueCallsNumber);
            Assert.Equal("value", objectBuilder.SetUnboundValueArgument(0));
        }

        [Fact]
        public void Given_option_group_calls_HandleImplicitOption()
        {
            typeParser.Parse(GetTokens(new OptionsGroupToken(new[] { new ShortOptionToken('a'), new ShortOptionToken('b') })));
            Assert.Equal(2, objectBuilder.HandleImplicitOptionCallsNumber);
        }
    }
}
