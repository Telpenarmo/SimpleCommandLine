using System;
using System.Collections.Generic;
using System.Linq;
using SimpleCommandLine.Tokenization.Tokens;

namespace SimpleCommandLine.Parsing
{
    internal class TokensParser : ITokensParser
    {
        private readonly IObjectBuilder builder;
        
        public TokensParser(IObjectBuilder builder)
        {
            this.builder = builder ?? throw new ArgumentNullException(nameof(builder));
        }

        public object Parse(IEnumerable<IArgumentToken> tokens)
        {
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

            if (builder.LastOption?.RequiresValue ?? false)
                throw new ArgumentException("Value was not provided for a token.");

            return builder.Parse();
        }

        protected void HandleOption(IOptionToken token)
        {
            if (builder.LastOption?.RequiresValue ?? false)
                throw new ArgumentException("Value was not provided for a token.");
            else
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
    }
}