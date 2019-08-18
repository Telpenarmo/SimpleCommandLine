using SimpleCommandLine.Registration;
using SimpleCommandLine.Tokenization.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleCommandLine.Parsing
{
    internal class ObjectBuilder : IObjectBuilder
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
            this.typeInfo = typeInfo ?? throw new ArgumentNullException(nameof(typeInfo));
            this.convertersFactory = convertersFactory ?? throw new ArgumentNullException(nameof(convertersFactory));
            this.formatProvider = formatProvider ?? throw new ArgumentNullException(nameof(formatProvider));
            objectResult = typeInfo.Factory.DynamicInvoke();
            maxValuesNumber = typeInfo.Values.Count();
            if (typeInfo.Values.Last().IsCollection)
                maxValuesNumber += typeInfo.Values.Last().Maximum;
        }

        private ParsingOptionInfo GetOptionInfo(IOptionToken token)
            => typeInfo.GetMatchingOptionInfo(token)
                ?? throw new ArgumentException($"This type does not contain the {token.ToString()} option.", nameof(token));

        public IOptionParser LastOption => assignedOptions.LastOrDefault();

        public bool AwaitsValue => usedValuesNumber < maxValuesNumber;

        public void AddOption(IOptionToken token)
        {
            var info = GetOptionInfo(token);
            if (info.IsCollection)
                assignedOptions.Add(new CollectionOptionParser(info, (CollectionConverter)info.ChooseConverter(convertersFactory), token));
            else if (assignedOptions.Exists(p => p.OptionToken.Equals(token)))
                throw new ArgumentException("This option was already declared.");
            else if (info.IsImplicit)
                assignedOptions.Add(new ImplicitOptionParser(info, (IValueConverter)info.ChooseConverter(convertersFactory), token));
            else
                assignedOptions.Add(new SingleValueOptionParser(info, (IValueConverter)info.ChooseConverter(convertersFactory), token));
        }

        public void AddValue(ValueToken token)
        {
            var info = typeInfo.GetValueInfoAt(usedValuesNumber) ?? typeInfo.Values.Last();
            if (!(!info.IsCollection || assignedValues.Last() is CollectionParser))
                assignedValues.Add(new CollectionParser(info, (CollectionConverter)info.ChooseConverter(convertersFactory)));
            else
                assignedValues.Add(new SingleValueParser(info, (IValueConverter)info.ChooseConverter(convertersFactory)));
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