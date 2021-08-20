using System;
using System.Collections.Generic;
using System.Linq;
using SimpleCommandLine.Tests.Fakes;
using SimpleCommandLine.Tokenizers;
using SimpleCommandLine.Tokens;
using Xunit;

namespace SimpleCommandLine.Tests.Tokenization.Tokenizers
{
    public class CommandTokenizerTests
    {
        private static CommandTokenizer GetTokenizerWithNext()
            => new(GetCommands().Select(x => x.Single() as string)) { Next = new FakeTokenizer() };

        private static CommandTokenizer GetTokenizerWithoutNext()
            => new(GetCommands().Select(x => x.Single() as string));

        [Theory, MemberData(nameof(GetCommands))]
        public void Given_Next_When_command_Then_returns_correct_CommandToken(string arg)
        {
            var tokenizer = GetTokenizerWithNext();
            var result = tokenizer.TokenizeArgument(arg);
            Assert.IsType<CommandToken>(result);
            Assert.Equal(arg, (result as CommandToken).Name);
            Assert.False((tokenizer.Next as FakeTokenizer).Invoked);
        }

        [Fact]
        public void Given_Next_When_non_command_Then_invokes_Next()
        {
            var tokenizer = GetTokenizerWithNext();
            var result = tokenizer.TokenizeArgument("arg");
            Assert.IsNotType<CommandToken>(result);
            Assert.True((tokenizer.Next as FakeTokenizer).Invoked);
        }

        [Fact]
        public void Given_Next_When_non_command_Then_returns_ValueToken()
        {
            var tokenizer = GetTokenizerWithoutNext();
            var result = tokenizer.TokenizeArgument("arg");
            Assert.IsType<ValueToken>(result);
            Assert.Equal("arg", (result as ValueToken).Value);
        }

        public static IEnumerable<object[]> GetCommands()
        {
            yield return new[] { "add" };
            yield return new[] { "command" };
            yield return new[] { "123" };
            yield return new[] { "a" };
            yield return new[] { "!2$" };
        }
    }
}
