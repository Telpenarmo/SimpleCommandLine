using System.Collections.Generic;
using SimpleCommandLine.Parsing;
using SimpleCommandLine.Tokenization.Tokens;

namespace SimpleCommandLine.Tests.Fakes
{
    class FakeObjectBuilder : IObjectBuilder
    {
        public object Build()
        {
            return null;
        }

        private List<IOptionToken> handleImplicitOptionArguments = new List<IOptionToken>();
        public void HandleImplicitOption(IOptionToken token)
        {
            handleImplicitOptionArguments.Add(token);
        }
        public int HandleImplicitOptionCallsNumber => handleImplicitOptionArguments.Count;
        public IOptionToken HandleImplicitOptionArgument(int callNumber) => handleImplicitOptionArguments[callNumber];

        private List<(IOptionToken, string)> setBoundValueArguments = new List<(IOptionToken, string)>();
        public void SetBoundValue(IOptionToken token, string value)
        {
            setBoundValueArguments.Add((token, value));
        }
        public int SetBoundValueCallsNumber => setBoundValueArguments.Count;
        public (IOptionToken, string) SetBoundValueArgument(int callNumber) => setBoundValueArguments[callNumber];

        private List<string> setUnboundValueArguments = new List<string>();
        public void SetUnboundValue(string value)
        {
            setUnboundValueArguments.Add(value);
        }
        public int SetUnboundValueCallsNumber => setUnboundValueArguments.Count;
        public string SetUnboundValueArgument(int callNumber) => setUnboundValueArguments[callNumber];

        public void Reset()
        {
            handleImplicitOptionArguments.Clear();
            setBoundValueArguments.Clear();
            setUnboundValueArguments.Clear();
        }
    }
}
