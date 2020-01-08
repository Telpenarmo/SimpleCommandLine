using System;
using System.Collections.Generic;
using SimpleCommandLine.Tokenization.Tokenizers;
using SimpleCommandLine.Tokenization.Tokens;
using Xunit;
using SimpleCommandLine.Tests.Fakes;

namespace SimpleCommandLine.Tests.Tokenization.Tokenizers
{
    public class AssignedValueTokenizerTests
    {
        private AssignedValueTokenizer GetTokenizer()
            => new AssignedValueTokenizer(new[] { ':' }, new FakeOptionTokenizer(), new ValueTokenizer()) { Next = new TokenizerFake() };

        private class FakeOptionTokenizer : ChainTokenizer
        {
            public override bool CanHandle(string arg) => arg == "valid_option";
            public override IArgumentToken Handle(string arg) => new LongOptionToken(arg);
        }

        [Fact]
        public void When_arg_contains_separator_and_is_valid_option_Then_returns_AssignedValueToken()
        {
            var tokenizer = GetTokenizer();
            var result = tokenizer.TokenizeArgument("valid_option:some_value");
            Assert.IsType<AssignedValueToken>(result);
            var assignedValue = result as AssignedValueToken;
            Assert.Equal("valid_option", (assignedValue.Option as LongOptionToken).Value);
            Assert.Equal("some_value", (assignedValue.Value as ValueToken).Value);
            Assert.False((tokenizer.Next as TokenizerFake).Invoked);
        }

        [Fact]
        public void When_arg_contains_separator_and_is_not_valid_option_Then_invokes_Next()
        {
            var tokenizer = GetTokenizer();
            tokenizer.TokenizeArgument("invalid_option:some_value");
            Assert.True((tokenizer.Next as TokenizerFake).Invoked);
        }

        [Fact]
        public void When_arg_does_not_contain_separator_and_is_valid_option_Then_invokes_Next()
        {
            var tokenizer = GetTokenizer();
            tokenizer.TokenizeArgument("valid_option");
            Assert.True((tokenizer.Next as TokenizerFake).Invoked);
        }

        [Fact]
        public void When_arg_does_not_contain_separator_and_is_not_valid_option_Then_invokes_Next()
        {
            var tokenizer = GetTokenizer();
            tokenizer.TokenizeArgument("invalid_option");
            Assert.True((tokenizer.Next as TokenizerFake).Invoked);
        }
    }
}
