﻿using System;
using System.Collections.Generic;
using System.Linq;
using SimpleCommandLine.Tokenization.Tokens;

namespace SimpleCommandLine.Parsing
{
    internal class TokensParser : ITokensParser
    {
        private readonly Func<IObjectBuilder> builderFactory;
        private IObjectBuilder builder;
        
        public TokensParser(Func<IObjectBuilder> builderFactory)
        {
            this.builderFactory = builderFactory ?? throw new ArgumentNullException(nameof(builderFactory));
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
                    case AssignedValueToken assignedValueToken:
                        HandleAssignedValue(assignedValueToken);
                        break;
                    case IOptionToken option:
                        HandleOption(option);
                        break;
                    case IValueToken value:
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

        protected void HandleAssignedValue(AssignedValueToken token)
        {
            HandleOption(token.Option);
            builder.LastOption.AddValue(token.Value);
        }

        protected void HandleValue(IValueToken token)
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