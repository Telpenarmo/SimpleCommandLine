using System;
using System.Collections.Generic;
using SimpleCommandLine.Tests.Fakes;
using SimpleCommandLine.Tokenizers;
using SimpleCommandLine.Tokens;
using Xunit;

namespace SimpleCommandLine.Tests.Tokenization.Tokenizers
{
    public class AssignedValueTokenizerTests
    {
        private AssignedValueTokenizer GetTokenizer()
            => new(new[] { ':' }, new FakeOptionTokenizer(), new ValueTokenizer()) { Next = new FakeTokenizer() };

        private class FakeOptionTokenizer : IOptionTokenizer
        {
            public bool IsOption(string arg) => arg == "valid_option";
            public OptionToken ProduceOptionToken(string arg) => new(arg);
        }

        [Fact]
        public void When_arg_contains_separator_and_is_valid_option_Then_returns_AssignedValueToken()
        {
            var tokenizer = GetTokenizer();
            var result = tokenizer.TokenizeArgument("valid_option:some_value");
            Assert.IsType<AssignedValueToken>(result);
            var assignedValue = result as AssignedValueToken;
            Assert.Equal("valid_option", assignedValue.Option.Value);
            Assert.Equal("some_value", assignedValue.Value.Value);
            Assert.False((tokenizer.Next as FakeTokenizer).Invoked);
        }

        [Fact]
        public void When_arg_contains_separator_and_is_not_valid_option_Then_invokes_Next()
        {
            var tokenizer = GetTokenizer();
            tokenizer.TokenizeArgument("invalid_option:some_value");
            Assert.True((tokenizer.Next as FakeTokenizer).Invoked);
        }

        [Fact]
        public void When_arg_does_not_contain_separator_and_is_valid_option_Then_invokes_Next()
        {
            var tokenizer = GetTokenizer();
            tokenizer.TokenizeArgument("valid_option");
            Assert.True((tokenizer.Next as FakeTokenizer).Invoked);
        }

        [Fact]
        public void When_arg_does_not_contain_separator_and_is_not_valid_option_Then_invokes_Next()
        {
            var tokenizer = GetTokenizer();
            tokenizer.TokenizeArgument("invalid_option");
            Assert.True((tokenizer.Next as FakeTokenizer).Invoked);
        }
    }
}
