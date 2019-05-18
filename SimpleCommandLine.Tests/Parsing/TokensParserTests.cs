using System;
using System.Collections.Generic;
using Xunit;
using SimpleCommandLine.Parsing;
using SimpleCommandLine.Tests.Fakes;
using SimpleCommandLine.Tokenization.Tokens;
using SimpleCommandLine.Parsing.Converters;

namespace SimpleCommandLine.Tests.Parsing
{
    public class TokensParserTests
    {
        private TokensParser GetTypeParser =>
            new TokensParser(new Registration.ParsingTypeInfo(TestObject.ValueInfos, TestObject.OptionsInfos, new Func<TestObject>(() => new TestObject())), CreateValuesConvertersFactory());

        private IValueConvertersFactory CreateValuesConvertersFactory()
        {
            var result = new ValueConvertersFactory();
            result.Register(new StringValueConverter(), typeof(string));
            result.Register(new IntValueConverter(), typeof(int));
            result.Register(new BoolValueConverter(), typeof(bool));
            return result;
        }

        [Fact]
        public void Given_one_value_sets_first()
        {
            var result = GetTypeParser.Parse(GetTokens(new ValueToken("content")));
            Assert.IsType<TestObject>(result);
            Assert.Equal("content", (result as TestObject).StringValue);
        }

        [Fact]
        public void Given_stringOption_and_value_sets_corresponding()
        {
            var result = GetTypeParser.Parse(GetTokens(new ShortOptionToken('s'), new ValueToken("content")));
            Assert.IsType<TestObject>(result);
            Assert.Equal("content", (result as TestObject).StringOption);
        }

        [Fact]
        public void Given_boolOption_sets_corresponding()
        {
            var result = GetTypeParser.Parse(GetTokens(new ShortOptionToken('1')));
            Assert.IsType<TestObject>(result);
            Assert.True((result as TestObject).BoolOption1);
            Assert.False((result as TestObject).BoolOption2);
            Assert.False((result as TestObject).BoolOption3);
        }

        [Fact]
        public void Given_boolOptionGroup_and_value_sets_corresponding()
        {
            var result = GetTypeParser.Parse(
                GetTokens(new OptionsGroupToken(new[] { new ShortOptionToken('1'), new ShortOptionToken('2'), new ShortOptionToken('s') }), new ValueToken("content")));
            Assert.IsType<TestObject>(result);
            Assert.True((result as TestObject).BoolOption1);
            Assert.True((result as TestObject).BoolOption2);
            Assert.Equal("content", (result as TestObject).StringOption);
            Assert.False((result as TestObject).BoolOption3);
        }

        [Fact]
        public void Given_stringOption_and_two_values_sets_corresponding()
        {
            var result = GetTypeParser.Parse(GetTokens(new ShortOptionToken('s'), new ValueToken("first"), new ValueToken("second")));
            Assert.IsType<TestObject>(result);
            Assert.Equal("first", (result as TestObject).StringOption);
            Assert.Equal("second", (result as TestObject).StringValue);
        }

        [Fact]
        public void Given_stringOption_throws()
        {
            Assert.Throws<ArgumentException>(() => GetTypeParser.Parse(GetTokens(new ShortOptionToken('s'))));
        }

        [Fact]
        public void Given_too_many_values_throws()
        {
            Assert.Throws<ArgumentException>(() => GetTypeParser.Parse(GetTokens(new ValueToken("content"), new ValueToken("12"), new ValueToken("wrong"))));
        }

        [Fact]
        public void Given_unknown_option_throws()
        {
            Assert.Throws<ArgumentException>(() => GetTypeParser.Parse(GetTokens(new LongOptionToken("void"), new ValueToken("wrong"))));
        }

        [Fact]
        public void Given_the_same_option_twice_throws()
        {
            Assert.Throws<ArgumentException>(()
                => GetTypeParser.Parse(GetTokens(new LongOptionToken("sOption"), new ValueToken("content"), new LongOptionToken("sOption"), new ValueToken("wrong"))));
        }
        private IEnumerable<IArgumentToken> GetTokens(params IArgumentToken[] arguments) => arguments;
    }
}
