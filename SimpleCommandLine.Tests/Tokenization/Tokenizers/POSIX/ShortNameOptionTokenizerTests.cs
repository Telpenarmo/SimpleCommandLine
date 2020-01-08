using System.Collections.Generic;
using SimpleCommandLine.Tests.Fakes;
using SimpleCommandLine.Tokenization.Tokenizers.POSIX;
using SimpleCommandLine.Tokenization.Tokens;
using Xunit;

namespace SimpleCommandLine.Tests.Tokenization.Tokenizers.POSIX
{
    public class ShortNameOptionTokenizerTests
    {
        private ShortNameOptionTokenizer GetTokenizerWithNext() => new ShortNameOptionTokenizer() { Next = new TokenizerFake() };
        private ShortNameOptionTokenizer GetTokenizerWithoutNext() => new ShortNameOptionTokenizer();

        [Fact]
        public void Given_Next_When_correct_string_Then_returns_correct_token()
        {
            var tokenizer = GetTokenizerWithNext();
            var result = tokenizer.TokenizeArgument("-a");
            Assert.IsType<ShortOptionToken>(result);
            Assert.Equal('a', (result as ShortOptionToken).Value);
            Assert.False((tokenizer.Next as TokenizerFake).Invoked);
        }

        [Theory, MemberData(nameof(GetWrongArguments))]
        public void Given_Next_When_wrong_string_Then_invokes_Next(string arg)
        {
            var tokenizer = GetTokenizerWithNext();
            var result = tokenizer.TokenizeArgument(arg);
            Assert.IsNotType<ShortOptionToken>(result);
            Assert.True((tokenizer.Next as TokenizerFake).Invoked);
            Assert.Equal(arg, (tokenizer.Next as TokenizerFake).Argument);
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