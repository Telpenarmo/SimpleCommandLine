﻿using System.Collections.Generic;
using SimpleCommandLine.Tests.Fakes;
using SimpleCommandLine.Tokenizers.POSIX;
using SimpleCommandLine.Tokens;
using Xunit;

namespace SimpleCommandLine.Tests.Tokenization.Tokenizers.POSIX
{
    public class ShortOptionTokenizerTests
    {
        private ShortOptionTokenizer GetTokenizerWithNext() => new ShortOptionTokenizer() { Next = new FakeTokenizer() };
        private ShortOptionTokenizer GetTokenizerWithoutNext() => new ShortOptionTokenizer();

        [Fact]
        public void Given_Next_When_correct_string_Then_returns_correct_token()
        {
            var tokenizer = GetTokenizerWithNext();
            var result = tokenizer.TokenizeArgument("-a");
            Assert.IsType<OptionToken>(result);
            Assert.Equal("a", (result as OptionToken).Value);
            Assert.False((tokenizer.Next as FakeTokenizer).Invoked);
        }

        [Theory, MemberData(nameof(GetWrongArguments))]
        public void Given_Next_When_wrong_string_Then_invokes_Next(string arg)
        {
            var tokenizer = GetTokenizerWithNext();
            var result = tokenizer.TokenizeArgument(arg);
            Assert.IsNotType<OptionToken>(result);
            Assert.True((tokenizer.Next as FakeTokenizer).Invoked);
            Assert.Equal(arg, (tokenizer.Next as FakeTokenizer).Argument);
        }

        [Theory, MemberData(nameof(GetWrongArguments))]
        public void Given_no_Next_When_wrong_string_Then_returns_ValueToken(string arg)
        {
            var tokenizer = GetTokenizerWithoutNext();
            var result = tokenizer.TokenizeArgument(arg);
            Assert.IsType<ValueToken>(result);
            Assert.Equal(arg, (result as ValueToken).Value);
        }

        public static IEnumerable<object[]> GetWrongArguments()
        {
            yield return new[] { "-aa" };
            yield return new[] { "--a" };
            yield return new[] { "a" };
            yield return new[] { "-" };
            yield return new[] { "aa" };
        }
    }
}