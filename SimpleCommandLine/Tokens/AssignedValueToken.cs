namespace SimpleCommandLine.Tokens
{
    public class AssignedValueToken : IArgumentToken
    {
        public AssignedValueToken(OptionToken option, ValueToken value)
        {
            Option = option;
            Value = value;
        }
        
        public OptionToken Option { get; }
        public ValueToken Value { get; }

        public bool Equals(IArgumentToken other)
            => other is AssignedValueToken token && token.Option.Equals(Option) && token.Value.Equals(Value);
    }
}