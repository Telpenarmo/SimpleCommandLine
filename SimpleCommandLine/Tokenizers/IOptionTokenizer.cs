using SimpleCommandLine.Tokens;

namespace SimpleCommandLine.Tokenizers
{
    public interface IOptionTokenizer
    {
        bool IsOption(string arg);
        OptionToken ProduceOptionToken(string arg);
    }
}