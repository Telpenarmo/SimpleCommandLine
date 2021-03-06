namespace SimpleCommandLine.Tokenization.Tokens
{
    public class AssignedValueToken : IArgumentToken
    {
        public AssignedValueToken(IOptionToken option, IValueToken value)
        {
            Option = option;
            Value = value;
        }
        
        public IOptionToken Option { get; }
        public IValueToken Value { get; }

        public bool Equals(IArgumentToken other)
            => other is AssignedValueToken token && token.Option.Equals(Option) && token.Value.Equals(Value);
    }
}