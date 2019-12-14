using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using SimpleCommandLine.Tokenization.Tokenizers;
using SimpleCommandLine.Tokenization.Tokens;
using SimpleCommandLine.Tests.Fakes;

namespace SimpleCommandLine.Tests.Tokenization.Tokenizers
{
    public class ValuesGroupTokenizerTests
    {
        public ValuesGroupTokenizer Tokenizer => new ValuesGroupTokenizer(new []{';', ','}){Next = new TokenizerFake()};

        [Fact]
        public void Given_single_word_invokes_Next()
        {
            var tokenizer = Tokenizer;
            var result = tokenizer.TokenizeArgument("value");
            Assert.IsNotType<ValuesGroupToken>(result);
            Assert.True((tokenizer.Next as TokenizerFake).Invoked);
            Assert.Equal("value", (tokenizer.Next as TokenizerFake).Argument);
        }

        [Fact]
        public void Given_two_words_returns_two_values()
        {
            var result = Tokenizer.TokenizeArgument("first;second");
            Assert.IsType<ValuesGroupToken>(result);

            var tokenResult = result as ValuesGroupToken;
            Assert.Equal(2, tokenResult.Tokens.Count);
            Assert.Equal(new ValueToken("first"), tokenResult.Tokens[0]);
            Assert.Equal(new ValueToken("second"), tokenResult.Tokens[1]);
        }

        [Fact]
        public void Given_single_word_with_two_nested_returns_one_group_with_two_members()
        {
            var result = Tokenizer.TokenizeArgument("first,second");
            Assert.IsType<ValuesGroupToken>(result);

            var tokenResult = result as ValuesGroupToken;
            Assert.IsType<ValuesGroupToken>(tokenResult);
            Assert.Equal(1, tokenResult.Tokens.Count);

            var member = tokenResult.Tokens[0] as ValuesGroupToken;
            Assert.Equal(2, member.Tokens.Count);
            Assert.Equal(new ValueToken("first"), member.Tokens[0]);
            Assert.Equal(new ValueToken("second"), tokenResult.Tokens[1]);
        }

        [Fact]
        public void Given_two_words_with_two_nested_each_returns_two_groups_with_two_members()
        {
            var result = Tokenizer.TokenizeArgument("first,second;third,fourth");
            Assert.IsType<ValuesGroupToken>(result);

            var tokenResult = result as ValuesGroupToken;
            Assert.IsType<ValuesGroupToken>(tokenResult);
            Assert.Equal(2, tokenResult.Tokens.Count);
            
            var member = tokenResult.Tokens[0] as ValuesGroupToken;
            Assert.Equal(2, member.Tokens.Count);
            Assert.Equal(new ValueToken("first"), member.Tokens[0]);
            Assert.Equal(new ValueToken("second"), tokenResult.Tokens[1]);
            
            member = tokenResult.Tokens[1] as ValuesGroupToken;
            Assert.Equal(2, member.Tokens.Count);
            Assert.Equal(new ValueToken("third"), member.Tokens[0]);
            Assert.Equal(new ValueToken("fourth"), tokenResult.Tokens[1]);
        }

        [Fact]
        public void Given_single_word_and_separator_returns_two_members()
        {
            var result = Tokenizer.TokenizeArgument("value;");
            Assert.IsType<ValuesGroupToken>(result);

            var tokenResult = result as ValuesGroupToken;
            Assert.Equal(2, tokenResult.Tokens.Count);
            Assert.Equal(new ValueToken("value"), tokenResult.Tokens[0]);
            Assert.Equal(new ValueToken(string.Empty), tokenResult.Tokens[1]);
        } 
    }
}