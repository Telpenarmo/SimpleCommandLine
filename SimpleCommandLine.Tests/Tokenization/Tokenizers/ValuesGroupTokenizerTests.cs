using System;
using System.Collections.Generic;
using System.Linq;
using SimpleCommandLine.Tests.Fakes;
using SimpleCommandLine.Tokenizers;
using SimpleCommandLine.Tokens;
using Xunit;

namespace SimpleCommandLine.Tests.Tokenization.Tokenizers
{
    public class ValuesGroupTokenizerTests
    {
        public static ValuesGroupTokenizer Tokenizer => new(new[] { ';', ',', '&' }) { Next = new FakeTokenizer() };

        [Fact]
        public void Given_single_word_invokes_Next()
        {
            var tokenizer = Tokenizer;
            var result = tokenizer.TokenizeArgument("value");
            Assert.IsNotType<ValuesGroupToken>(result);
            Assert.True((tokenizer.Next as FakeTokenizer).Invoked);
            Assert.Equal("value", (tokenizer.Next as FakeTokenizer).Argument);
        }

        [Fact]
        public void Given_two_words_returns_two_values()
        {
            var result = Tokenizer.TokenizeArgument("first;second");
            Assert.IsType<ValuesGroupToken>(result);

            var tokenResult = result as ValuesGroupToken;
            Assert.Equal(2, tokenResult.Tokens.Count);
            Assert.Equal("first", tokenResult.Tokens[0].ToString());
            Assert.Equal("second", tokenResult.Tokens[1].ToString());
        }

        [Fact]
        public void Given_single_word_with_two_nested_returns_one_group_with_two_members()
        {
            var result = Tokenizer.TokenizeArgument("first,second");
            Assert.IsType<ValuesGroupToken>(result);
            var tokenResult = result as ValuesGroupToken;
            Assert.IsType<ValuesGroupToken>(tokenResult);
            Assert.Equal(2, tokenResult.Tokens.Count);
            Assert.Equal("first", tokenResult.Tokens[0].ToString());
            Assert.Equal("second", tokenResult.Tokens[1].ToString());
        }

        [Fact]
        public void Given_words_with_double_nested_returns_nested_groups()
        {
            var result = Tokenizer.TokenizeArgument("first&second,third,fourth&fifth&sixth,;seventh&eighth");
            Assert.IsType<ValuesGroupToken>(result);

            var tokenResult = result as ValuesGroupToken;
            Assert.IsType<ValuesGroupToken>(tokenResult);
            Assert.Equal(2, tokenResult.Tokens.Count);

            // first&second,third,fourth&fifth&sixth,
            var member = tokenResult.Tokens[0] as ValuesGroupToken;
            Assert.Equal(4, member.Tokens.Count);
            // first&second
            var subMember = member.Tokens[0] as ValuesGroupToken;
            Assert.Equal(2, subMember.Tokens.Count);
            Assert.Equal("first", subMember.Tokens[0].ToString());
            Assert.Equal("second", subMember.Tokens[1].ToString());
            // third
            Assert.Equal("third", member.Tokens[1].ToString());
            // fourth&fifth&sixth
            subMember = member.Tokens[2] as ValuesGroupToken;
            Assert.Equal(3, subMember.Tokens.Count);
            Assert.Equal("fourth", subMember.Tokens[0].ToString());
            Assert.Equal("fifth", subMember.Tokens[1].ToString());
            Assert.Equal("sixth", subMember.Tokens[2].ToString());
            // null
            Assert.Equal(string.Empty, member.Tokens[3].ToString());

            // seven&eight
            member = tokenResult.Tokens[1] as ValuesGroupToken;
            Assert.Equal(2, member.Tokens.Count);
            Assert.Equal("seventh", member.Tokens[0].ToString());
            Assert.Equal("eighth", member.Tokens[1].ToString());
        }

        [Fact]
        public void Given_single_word_and_separator_returns_two_members()
        {
            var result = Tokenizer.TokenizeArgument("value;");
            Assert.IsType<ValuesGroupToken>(result);

            var tokenResult = result as ValuesGroupToken;
            Assert.Equal(2, tokenResult.Tokens.Count);
            Assert.Equal("value", tokenResult.Tokens[0].ToString());
            Assert.Equal(string.Empty, tokenResult.Tokens[1].ToString());
        }
    }
}