using System;
using System.Collections.Generic;
using SimpleCommandLine.Registration;
using SimpleCommandLine.Tokenization.Tokens;

namespace SimpleCommandLine.Parsing
{
    internal class ObjectBuilder
    {
        private readonly ParsingTypeInfo typeInfo;
        private readonly IConvertersFactory convertersFactory;
        private readonly List<IOptionParser> assignedOptions = new List<IOptionParser>();
        private readonly List<IArgumentParser> assignedValues = new List<IArgumentParser>();
        private readonly object result;
        private readonly IFormatProvider formatProvider;
        private int usedValuesNumber;
        private readonly int maxValuesNumber;

        public ObjectBuilder(ParsingTypeInfo typeInfo, IConvertersFactory convertersFactory, IFormatProvider formatProvider)
        {
            this.typeInfo = typeInfo;
            this.convertersFactory = convertersFactory;
            this.formatProvider = formatProvider;
            result = typeInfo.Factory.DynamicInvoke();
            maxValuesNumber = AllValuesNumber;
            if (LastValue.IsCollection)
                maxValuesNumber += LastValue.Maximum - 1;
        }

        private ParsingOptionInfo GetOptionInfo(OptionToken token)
            => typeInfo.GetMatchingOptionInfo(token)
                ?? throw new ArgumentException($"This type does not contain the {token} option.", nameof(token));

        private int AllValuesNumber => typeInfo.Values.Count;
        private ParsingValueInfo LastValue => typeInfo.Values[maxValuesNumber - 1];

        public IOptionParser LastAssignedOption => assignedOptions[assignedOptions.Count - 1];
        public IArgumentParser LastAssignedValue => assignedValues[assignedValues.Count - 1];

        public bool AwaitsValue => usedValuesNumber < maxValuesNumber;

        public void AddOption(OptionToken token)
        {
            var info = GetOptionInfo(token);
            if (info.IsCollection)
                assignedOptions.Add(new CollectionOptionParser(info, GetConverter<CollectionConverter>(info), token));
            else if (assignedOptions.Exists(p => p.OptionToken.Equals(token)))
                throw new ArgumentException("This option was already declared.");
            else if (info.IsImplicit)
                assignedOptions.Add(new ImplicitOptionParser(info, GetConverter<IValueConverter>(info), token));
            else
                assignedOptions.Add(new SingleValueOptionParser(info, GetConverter<IValueConverter>(info), token));
        }

        private T GetConverter<T>(ParsingOptionInfo info) where T : class, IConverter
            => info.ChooseConverter(convertersFactory) as T;

        public void AddValue(ValueToken token)
        {
            var info = usedValuesNumber < AllValuesNumber ? typeInfo.Values[usedValuesNumber] : LastValue;
            if (!info.IsCollection || LastAssignedValue is CollectionParser)
                assignedValues.Add(new SingleValueParser(info, info.ChooseConverter(convertersFactory) as IValueConverter));
            else
                assignedValues.Add(new CollectionParser(info, info.ChooseConverter(convertersFactory) as CollectionConverter));
            LastAssignedValue.AddValue(token);
            usedValuesNumber++;
        }

        public object Parse()
        {
            assignedOptions.ForEach(o => o.Parse(result, formatProvider));
            assignedValues.ForEach(o => o.Parse(result, formatProvider));
            return result;
        }
    }
}