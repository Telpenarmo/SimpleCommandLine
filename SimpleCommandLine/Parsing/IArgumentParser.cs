namespace SimpleCommandLine.Parsing
{
    internal interface IArgumentParser
    {
        void AddValue(Tokenization.Tokens.IValueToken token);
        void Parse(object target, System.IFormatProvider formatProvider);
    }
}