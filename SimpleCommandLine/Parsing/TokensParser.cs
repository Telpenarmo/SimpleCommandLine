using System;
using System.Collections.Generic;
using System.Linq;
using SimpleCommandLine.Registration;
using SimpleCommandLine.Tokenization.Tokens;

namespace SimpleCommandLine.Parsing
{
    internal class TokensParser : ITokensParser
    {
        //private readonly IObjectBuilder objectBuilder;
        private readonly ParsingTypeInfo typeInfo;
        private readonly IValueConvertersFactory convertersFactory;
        private IOptionToken awaiting;
        private int usedValuesNumber;
        private readonly IFormatProvider formatProvider;
        private readonly object objectResult;
        private Dictionary<IArgumentToken, ParsingArgumentInfo> AssignedArguments { get; } = new Dictionary<IArgumentToken, ParsingArgumentInfo>();

        private ParsingOptionInfo GetOptionInfo(IOptionToken token)
            => typeInfo.GetMatchingOptionInfo(token)
                ?? throw new ArgumentException($"This type does not contain the {token.ToString()} option.", nameof(token));

        public TokensParser(ParsingTypeInfo typeInfo, IValueConvertersFactory convertersFactory)
        {
            //this.objectBuilder = objectBuilder ?? throw new ArgumentNullException(nameof(objectBuilder));
            this.typeInfo = typeInfo ?? throw new ArgumentNullException(nameof(typeInfo));
            this.convertersFactory = convertersFactory ?? throw new ArgumentNullException(nameof(convertersFactory));
            objectResult = typeInfo.Factory.DynamicInvoke();
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
                        HandleOptionGroup(optionsGroup);
                        break;
                    case IOptionToken option:
                        HandleOption(option);
                        break;
                    case ValueToken value:
                        HandleValue(value);
                        break;
                }
            }

            if (awaiting != null)
                throw new ArgumentException("Value was not provided for a token.");

            return objectResult;
        }

        protected void HandleOption(IOptionToken token)
        {
            if (awaiting != null)
            {
                if (GetOptionInfo(awaiting).IsCollection && AssignedArguments.Where(o => o.Value == awaiting).Count() > GetOptionInfo(awaiting).Minimum)
                {
                    awaiting = token;
                    return;
                }
                throw new ArgumentException("Value was not provided for a token.");
            }

            var currentOption = GetOptionInfo(token);
            if (currentOption.IsImplicit)
                SetPropertyValue(currentOption, string.Empty);
            else
                awaiting = token;
        }

        private void HandleOptionGroup(OptionsGroupToken group)
        {
            foreach (var option in group.Tokens)
                HandleOption(option);
        }

        protected void HandleValue(ValueToken token)
        {
            if (awaiting != null)
            {
                SetPropertyValue(GetOptionInfo(awaiting), token.Value);
                if (! GetOptionInfo(awaiting).IsCollection)
                    awaiting = null;
            }
            else
            {
                var currentValue = typeInfo.GetValueInfoAt(usedValuesNumber);
                if (currentValue == null)
                    throw new ArgumentException("This type does not accept any more values.");
                SetPropertyValue(currentValue, token.Value);
                if (! currentValue.IsCollection || AssignedArguments.Where(o => o.Value == currentValue).Count() == currentValue.Maximum)
                    usedValuesNumber++;
            }
        }

        private void SetPropertyValue(ParsingArgumentInfo argumentInfo, string s)
        {
            object value;
            var converter = convertersFactory.GetConverter(argumentInfo);
            try
            {
                value = converter.Convert(s, formatProvider);
            }
            catch (FormatException ex)
            {
                throw new InvalidOperationException("Given value can not be assigned to specified option.", ex);
            }
            argumentInfo.SetValue(objectResult, value);

        }
    }
}