using System;
using System.Collections.Generic;
using SimpleCommandLine.Tokenization.Tokenizers;
using SimpleCommandLine.Tokenization.Tokenizers.POSIX;
using Xunit;

namespace SimpleCommandLine.Tests.Tokenization.Tokenizers.POSIX
{
    public class POSIXTokenizerBuilderTests
    {
        private POSIXTokenizerBuilder GetBuilder(bool allowAssigningOptions, bool allowShortOptionGroups, char[] separators)
            => new POSIXTokenizerBuilder() { AllowAssigningOptions = allowAssigningOptions, AllowShortOptionGroups = allowShortOptionGroups, Separators = separators };

        [Fact]
        public void When_all_set_to_false_Then_builds_minimal_chain()
        {
            var builder = GetBuilder(false, false, Array.Empty<char>());

            var chain = builder.BuildTokenizer();

            Assert.IsType<ShortNameOptionTokenizer>(chain);
            chain = (chain as ShortNameOptionTokenizer).Next;
            Assert.IsType<LongNameOptionTokenizer>(chain);
            chain = (chain as LongNameOptionTokenizer).Next;
            Assert.IsType<ValueTokenizer>(chain);
        }

        [Fact]
        public void When_everything_set_Then_builds_maximal_chain_in_right_order()
        {
            var builder = GetBuilder(true, true, new[] { ':' });

            var chain = builder.BuildTokenizer();

            Assert.IsType<ShortNameOptionTokenizer>(chain);
            chain = (chain as ShortNameOptionTokenizer).Next;
            Assert.IsType<AssignedValueTokenizer>(chain);
            chain = (chain as AssignedValueTokenizer).Next;
            Assert.IsType<AssignedValueTokenizer>(chain);
            chain = (chain as AssignedValueTokenizer).Next;
            Assert.IsType<OptionsGroupTokenizer>(chain);
            chain = (chain as OptionsGroupTokenizer).Next;
            Assert.IsType<LongNameOptionTokenizer>(chain);
            chain = (chain as LongNameOptionTokenizer).Next;
            Assert.IsType<ValueTokenizer>(chain);
        }
    }
}
