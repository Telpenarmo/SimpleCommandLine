using System;
using System.Collections.Generic;
using System.Linq;
using SimpleCommandLine.Registration;
using SimpleCommandLine.Tokenization.Tokens;

namespace SimpleCommandLine.Parsing
{
    internal class ObjectBuilder : IObjectBuilder
    {
        private readonly ParsingTypeInfo typeInfo;
        private readonly IValueConvertersFactory convertersFactory;
        private int usedValuesNumber = 0;
        private object objectResult;
        public IFormatProvider FormatProvider { get; set; } = System.Globalization.CultureInfo.CurrentCulture;

        private ParsingOptionInfo GetOptionInfoWithToken(IOptionToken token)
            => typeInfo.GetMatchingOptionInfo(token)
                ?? throw new ArgumentException($"This type does not contain the {token.ToString()} option.", nameof(token));

        public ObjectBuilder(ParsingTypeInfo typeInfo, IValueConvertersFactory convertersFactory)
        {
            this.typeInfo = typeInfo ?? throw new ArgumentNullException(nameof(typeInfo));
            this.convertersFactory = convertersFactory ?? throw new ArgumentNullException(nameof(convertersFactory));
            objectResult = typeInfo.Factory.DynamicInvoke(null);
        }

        public object Build()
        {
            var result = objectResult;
            Reset();
            return result;
        }

        public void SetBoundValue(IOptionToken token, string value)
        {
            if (token == null)
                throw new ArgumentNullException(nameof(token));
                
            SetPropertyValue(GetOptionInfoWithToken(token), value);
        }

        public void SetUnboundValue(string value)
        {
            var valueInfo = typeInfo.GetValueInfoAt(usedValuesNumber);
            if (value != null)
            {
                SetPropertyValue(valueInfo, value);
                usedValuesNumber++;
            }
            else
                throw new InvalidOperationException("This type does not accept any more values.");
        }

        public void HandleImplicitOption(IOptionToken token)
        {
            SetPropertyValue(GetOptionInfoWithToken(token), string.Empty);
        }

        private void SetPropertyValue(ParsingArgumentInfo argumentInfo, string s)
        {
            object value;
            var converter = convertersFactory.GetConverter(argumentInfo);
            try
            {
                value = converter.Convert(s, FormatProvider);
            }
            catch (FormatException ex)
            {
                throw new InvalidOperationException("Given value can not be assigned to specified option.", ex);
            }
            argumentInfo.SetValue(objectResult, value);
        }

        private void Reset()
        {
            objectResult = typeInfo.Factory.DynamicInvoke(null);
            usedValuesNumber = 0;
        }
    }
}