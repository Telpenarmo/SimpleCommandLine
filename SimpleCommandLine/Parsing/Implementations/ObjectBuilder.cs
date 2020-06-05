using SimpleCommandLine.Registration;
using SimpleCommandLine.Tokenization.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleCommandLine.Parsing
{
    internal class ObjectBuilder
    {
        private readonly ParsingTypeInfo typeInfo;
        private readonly IConvertersFactory convertersFactory;
        private readonly List<IOptionParser> assignedOptions = new List<IOptionParser>();
        private readonly List<IArgumentParser> assignedValues = new List<IArgumentParser>();
        private readonly object objectResult;
        private readonly IFormatProvider formatProvider;
        private int usedValuesNumber;
        private readonly int maxValuesNumber;

        public ObjectBuilder(ParsingTypeInfo typeInfo, IConvertersFactory convertersFactory, IFormatProvider formatProvider)
        {
            this.typeInfo = typeInfo;
            this.convertersFactory = convertersFactory;
            this.formatProvider = formatProvider;
            objectResult = typeInfo.Factory.DynamicInvoke();
            maxValuesNumber = typeInfo.Values.Count();
            if (typeInfo.Values.Last().IsCollection)
                maxValuesNumber += typeInfo.Values.Last().Maximum - 1;
        }

        private ParsingOptionInfo GetOptionInfo(OptionToken token)
            => typeInfo.GetMatchingOptionInfo(token)
                ?? throw new ArgumentException($"This type does not contain the {token} option.", nameof(token));

        public IOptionParser LastOption => assignedOptions.LastOrDefault();

        public bool AwaitsValue => usedValuesNumber < maxValuesNumber;

        public void AddOption(OptionToken token)
        {
            var info = GetOptionInfo(token);
            if (info.IsCollection)
                assignedOptions.Add(
                    new CollectionOptionParser(info, info.ChooseConverter(convertersFactory) as CollectionConverter, token));
            else if (assignedOptions.Exists(p => p.OptionToken.Equals(token)))
                throw new ArgumentException("This option was already declared.");
            else if (info.IsImplicit)
                assignedOptions.Add(
                    new ImplicitOptionParser(info, info.ChooseConverter(convertersFactory) as IValueConverter, token));
            else
                assignedOptions.Add(
                    new SingleValueOptionParser(info, info.ChooseConverter(convertersFactory) as IValueConverter, token));
        }

        public void AddValue(ValueToken token)
        {
            var info = typeInfo.GetValueInfoAt(usedValuesNumber) ?? typeInfo.Values.Last();
            if (!info.IsCollection || assignedValues.Last() is CollectionParser)
                assignedValues.Add(new SingleValueParser(info, (IValueConverter)info.ChooseConverter(convertersFactory)));
            else
                assignedValues.Add(new CollectionParser(info, (CollectionConverter)info.ChooseConverter(convertersFactory)));
            assignedValues.Last().AddValue(token);
            usedValuesNumber++;
        }

        public object Parse()
        {
            assignedOptions.ForEach(o => o.Parse(objectResult, formatProvider));
            assignedValues.ForEach(o => o.Parse(objectResult, formatProvider));
            return objectResult;
        }
    }
}