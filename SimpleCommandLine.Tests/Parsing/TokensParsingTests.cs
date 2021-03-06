﻿using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using SimpleCommandLine.Parsing;
using SimpleCommandLine.Tests.Fakes;
using SimpleCommandLine.Tokenization.Tokens;
using SimpleCommandLine.Parsing.Converters;
using static SimpleCommandLine.Parsing.Converters.StockConverters;
using static SimpleCommandLine.Parsing.Converters.NumericValueConverters;

namespace SimpleCommandLine.Tests.Parsing
{
    public class TokensParsingTests
    {
        private TokensParser GetTypeParser =>
            new TokensParser(() => new ObjectBuilder(
                new Registration.ParsingTypeInfo(TestObject.ValueInfos, TestObject.OptionsInfos, new Func<TestObject>(() => new TestObject())),
                CreateValuesConvertersFactory(),
                System.Globalization.CultureInfo.InvariantCulture));

        private IConvertersFactory CreateValuesConvertersFactory()
        {
            var result = new ConvertersFactory();
            result.RegisterConverter(StringConverter, typeof(string));
            result.RegisterConverter(Int32Converter, typeof(int));
            result.RegisterConverter(new BoolValueConverter(), typeof(bool));
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
        public void Given_boolOption_and_value_sets_corresponding()
        {
            var result = GetTypeParser.Parse(GetTokens(new ShortOptionToken('1'), new ValueToken("true")));
            Assert.IsType<TestObject>(result);
            Assert.True((result as TestObject).BoolOption1);
            Assert.False((result as TestObject).BoolOption2);
            Assert.False((result as TestObject).BoolOption3);
            Assert.Equal("true", (result as TestObject).StringValue);
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
            Assert.Throws<ArgumentException>(() => GetTypeParser.Parse(ProduceValueTokens("content", "12", "wrong")));
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

        [Fact]
        public void Given_arrayOption_and_valid_number_of_values_sets_corresponding()
        {
            var result = GetTypeParser.Parse(GetTokens(new ShortOptionToken('a')).Concat(ProduceValueTokens("first", "second", "third")));
            Assert.IsType<TestObject>(result);
            Assert.Equal(new[] { "first", "second", "third" }, (result as TestObject).ArrayOption);
        }

        [Fact]
        public void Given_listOption_and_valid_number_of_values_sets_corresponding()
        {
            var result = GetTypeParser.Parse(GetTokens(new ShortOptionToken('l')).Concat(ProduceValueTokens("first", "second", "third")));
            Assert.IsType<TestObject>(result);
            Assert.Equal(new List<string> { "first", "second", "third" }, (result as TestObject).ListOption);
        }

        [Fact]
        public void Given_arrayOption_and_maximal_number_of_values_and_value_sets_corresponding()
        {
            var result = GetTypeParser.Parse(GetTokens(
                new ShortOptionToken('a')).Concat(ProduceValueTokens("first", "second", "third", "fourth", "value")));
            Assert.IsType<TestObject>(result);
            Assert.Equal(new[] { "first", "second", "third", "fourth" }, (result as TestObject).ArrayOption);
            Assert.Equal("value", (result as TestObject).StringValue);
        }

        [Fact]
        public void Given_arrayOption_and_too_few_values_throws()
        {
            Assert.Throws<ArgumentException>(() => GetTypeParser.Parse(GetTokens(
                new ShortOptionToken('a')).Concat(ProduceValueTokens("first", "second"))));
        }

        [Fact]
        public void Given_arrayOption_and_valuesGroup_sets_corresponding()
        {
            var result = GetTypeParser.Parse(GetTokens(new ShortOptionToken('a'),
                new ValuesGroupToken(ProduceValueTokens("first", "second", "third"))));
            Assert.IsType<TestObject>(result);
            Assert.Equal(new[] { "first", "second", "third" }, (result as TestObject).ArrayOption);
        }

        [Fact]
        public void Given_arrayOption_and_too_small_valuesGroup_and_value_sets_corresponding()
        {
            var result = GetTypeParser.Parse(GetTokens(new ShortOptionToken('a'),
                new ValuesGroupToken(ProduceValueTokens("first", "second")), new ValueToken("third")));
            Assert.IsType<TestObject>(result);
            Assert.Equal(new[] { "first", "second", "third" }, (result as TestObject).ArrayOption);
        }

        [Fact]
        public void Given_arrayOption_and_too_small_valuesGroup_throws()
        {
            Assert.Throws<ArgumentException>(() => GetTypeParser.Parse(GetTokens(
                new ShortOptionToken('a'), new ValuesGroupToken(ProduceValueTokens("first", "second")))));
        }

        [Fact]
        public void Given_arrayOption_and_too_big_valuesGroup_throws()
        {
            Assert.Throws<ArgumentException>(() => GetTypeParser.Parse(GetTokens(
                new ShortOptionToken('a'), new ValuesGroupToken(ProduceValueTokens("first", "second", "third", "fourth", "fifth")))));
        }

        [Fact]
        public void Given_enumOption_and_valid_value_sets_corresponding()
        {
            var result = GetTypeParser.Parse(GetTokens(new ShortOptionToken('e'), new ValueToken("first")));
            Assert.IsType<TestObject>(result);
            Assert.Equal(AnEnum.first, (result as TestObject).EnumOption);
        }

        [Fact]
        public void Given_enumOption_and_invalid_value_throws()
        {
            Assert.Throws<FormatException>(() => GetTypeParser.Parse(GetTokens(new ShortOptionToken('e'), new ValueToken("invalid"))));
        }

        [Fact]
        public void Given_args_twice_processess_twice()
        {
            var parser = GetTypeParser;
            
            var firstResult = parser.Parse(GetTokens(new ShortOptionToken('1'), new ShortOptionToken('s'), new ValueToken("content"), new ValueToken("value")));
            Assert.True((firstResult as TestObject).BoolOption1);
            Assert.Equal("content", (firstResult as TestObject).StringOption);
            Assert.Equal("value", (firstResult as TestObject).StringValue);

            var secondResult = parser.Parse(GetTokens(new ShortOptionToken('s'), new ValueToken("content"), new ValueToken("value")));
            Assert.False((secondResult as TestObject).BoolOption1);
            Assert.Equal("content", (secondResult as TestObject).StringOption);
            Assert.Equal("value", (secondResult as TestObject).StringValue);

            Assert.NotSame(firstResult, secondResult);
        }

        private IEnumerable<IArgumentToken> GetTokens(params IArgumentToken[] arguments) => arguments;
        private IEnumerable<ValueToken> ProduceValueTokens(params string[] values) => values.Select(v => new ValueToken(v));
    }
}