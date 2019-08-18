namespace SimpleCommandLine.Parsing
{
    internal interface IArgumentParser
    {
        void AddValue(Tokenization.Tokens.ValueToken token);
        void Parse(object target, System.IFormatProvider formatProvider);
    }
}