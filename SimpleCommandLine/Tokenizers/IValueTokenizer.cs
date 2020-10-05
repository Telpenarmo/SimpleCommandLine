using SimpleCommandLine.Tokens;

namespace SimpleCommandLine.Tokenizers
{
    public interface IValueTokenizer
    {
        ValueToken ProduceValueToken(string arg);
    }
}