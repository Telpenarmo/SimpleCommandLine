using System;
using System.Collections.Generic;
using System.Linq;
using SimpleCommandLine.Registration;
using SimpleCommandLine.Tokenization.Tokens;

namespace SimpleCommandLine.Parsing
{
    internal class TokensParser : ITokensParser
    {
        private readonly IObjectBuilder objectBuilder;
        private IOptionToken awaiting;

        public TokensParser(IObjectBuilder objectBuilder)
        {
            this.objectBuilder = objectBuilder ?? throw new ArgumentNullException(nameof(objectBuilder));
        }

        public object Parse(IEnumerable<IArgumentToken> tokens)
        {
            var argumentsQueue = new Queue<IArgumentToken>(tokens);
            while (argumentsQueue.Any())
            {
                IArgumentToken current = argumentsQueue.Dequeue();
                if (current is IOptionToken option)
                    HandleOption(option);
                else HandleValue(current as ValueToken);
            }
            if (awaiting != null) // check if the last option was iplicitly declared
                objectBuilder.HandleImplicitOption(awaiting);
            return objectBuilder.Build();
        }

        protected void HandleOption(IOptionToken token)
        {
            if (awaiting == null)
            {
                if (token is OptionsGroupToken group)
                    HandleOptionGroup(group);
                else awaiting = token;
            }
            else
            {
                objectBuilder.HandleImplicitOption(awaiting);
                awaiting = token;
            }
        }

        private void HandleOptionGroup(OptionsGroupToken group)
        {
            foreach (var option in group.Tokens) HandleOption(option);
        }

        protected void HandleValue(ValueToken token)
        {
            if (awaiting != null)
            {
                objectBuilder.SetBoundValue(awaiting, token.Value);
                awaiting = null;
            }
            else objectBuilder.SetUnboundValue(token.Value);
        }
    }
}