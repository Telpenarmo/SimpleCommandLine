using System.Collections.Generic;
using System.Linq;
using SimpleCommandLine.Tests.Fakes;
using SimpleCommandLine.Tokenizers.POSIX;
using SimpleCommandLine.Tokens;
using Xunit;

namespace SimpleCommandLine.Tests.Tokenization.Tokenizers.POSIX
{
    public class OptionsGroupTokenizerTests
    {
        private OptionsGroupTokenizer GetTokenizerWithNext() => new OptionsGroupTokenizer() { Next = new FakeTokenizer() };
        private OptionsGroupTokenizer GetTokenizerWithoutNext() => new OptionsGroupTokenizer();
    
        [Fact]
        public void Given_Next_When_correct_string_Then_returns_correct_token()
        {
            var tokenizer = GetTokenizerWithNext();
            var result = tokenizer.TokenizeArgument("-abc");
            var expected = new List<OptionToken>
            {
                new OptionToken("a"),
                new OptionToken("b"),
                new OptionToken("c"),
            };
            Assert.IsType<OptionsGroupToken>(result);
            Assert.Collection((result as OptionsGroupToken).Tokens,
                x => expected[0].Value.Equals(x.Value),
                x => expected[1].Value.Equals(x.Value),
                x => expected[2].Value.Equals(x.Value));
            Assert.False((tokenizer.Next as FakeTokenizer).Invoked);
        }

        [Theory, MemberData(nameof(GetWrongArguments))]
        public void Given_Next_When_wrong_string_Then_invokes_Next(string arg)
        {
            var tokenizer = GetTokenizerWithNext();
            var result = tokenizer.TokenizeArgument(arg);
            Assert.IsNotType<OptionsGroupToken>(result);
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
            yield return new[] { "-a" };
            yield return new[] { "--a" };
            yield return new[] { "a" };
            yield return new[] { "-" };
            yield return new[] { "aa" };
        }
    }
}