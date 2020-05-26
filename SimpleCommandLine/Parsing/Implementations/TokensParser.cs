using System;
using System.Collections.Generic;
using System.Linq;
using SimpleCommandLine.Tokenization.Tokens;

namespace SimpleCommandLine.Parsing
{
    internal class TokensParser
    {
        private readonly Func<ObjectBuilder> builderFactory;
        private ObjectBuilder builder;
        
        public TokensParser(Func<ObjectBuilder> builderFactory)
        {
            this.builderFactory = builderFactory;
        }

        public object Parse(IEnumerable<IArgumentToken> tokens)
        {
            builder = builderFactory();

            var argumentsQueue = new Queue<IArgumentToken>(tokens);
            while (argumentsQueue.Any())
            {
                IArgumentToken current = argumentsQueue.Dequeue();
                switch (current)
                {
                    case OptionsGroupToken optionsGroup:
                        HandleOptionsGroup(optionsGroup);
                        break;
                    case IOptionToken option:
                        HandleOption(option);
                        break;
                    case ValueToken value:
                        HandleValue(value);
                        break;
                }
            }

            EnsureLastOptionSet();
            return builder.Parse();
        }

        protected void HandleOption(IOptionToken token)
        {
            EnsureLastOptionSet();
            builder.AddOption(token);
        }

        private void HandleOptionsGroup(OptionsGroupToken group)
        {
            foreach (var option in group.Tokens)
                HandleOption(option);
        }

        protected void HandleValue(ValueToken token)
        {
            if (builder.LastOption?.AcceptsValue ?? false)
                builder.LastOption.AddValue(token);
            else if (builder.AwaitsValue)
                builder.AddValue(token);
            else
                throw new ArgumentException("This type does not accept any more values.");
        }

        private void EnsureLastOptionSet()
        {
            if (builder.LastOption?.RequiresValue ?? false)
                throw new ArgumentException("Value was not provided for a token.");
        }
    }
}