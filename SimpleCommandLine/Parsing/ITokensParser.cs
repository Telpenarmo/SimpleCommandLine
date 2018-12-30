using System.Collections.Generic;

namespace SimpleCommandLine.Parsing
{
    internal interface ITokensParser
    {
        object Parse(IEnumerable<Tokenization.Tokens.IArgumentToken> tokens);
    }
}