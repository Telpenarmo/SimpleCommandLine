namespace SimpleCommandLine.Parsing
{
    internal interface ITokensParserFactory
    {
        ITokensParser Build();
        ITokensParser Build(string commandName);
    }
}