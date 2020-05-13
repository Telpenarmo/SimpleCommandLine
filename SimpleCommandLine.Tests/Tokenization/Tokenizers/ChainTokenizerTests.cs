using SimpleCommandLine.Tokenization.Tokenizers;
using SimpleCommandLine.Tokenization.Tokens;
using Xunit;

namespace SimpleCommandLine.Tests.Tokenization.Tokenizers
{
    public class ChainTokenizerTests
    {
        private class FakeTokenizer : ChainTokenizer
        {
            public override bool CanHandle(string arg) => true;
            public override IArgumentToken Handle(string arg) => null;
        }

        [Fact]
        public void When_Next_set_to_non_ChainTokenizer_Add_inserts_into_middle()
        {
            var chain = new FakeTokenizer { Next = new ValueTokenizer() };
            chain.AppendLink(new FakeTokenizer());
            Assert.IsType<FakeTokenizer>(chain.Next);
            Assert.IsType<ValueTokenizer>((chain.Next as ChainTokenizer).Next);
        }
    }
}