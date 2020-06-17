﻿using System;
using System.Collections.Generic;
using SimpleCommandLine.Registration;
using SimpleCommandLine.Tokenization.Tokens;

namespace SimpleCommandLine.Parsing
{
    internal class ObjectBuilder
    {
        private readonly TypeInfo typeInfo;
        private readonly ConvertersFactory convertersFactory;
        private readonly List<IArgumentParser> assignedOptions = new List<IArgumentParser>();
        private readonly List<IArgumentParser> assignedValues = new List<IArgumentParser>();
        private readonly object result;
        private readonly IFormatProvider formatProvider;
        private int usedValuesNumber;
        private readonly int maxValuesNumber;

        public ObjectBuilder(TypeInfo typeInfo, ConvertersFactory convertersFactory, IFormatProvider formatProvider)
        {
            this.typeInfo = typeInfo;
            this.convertersFactory = convertersFactory;
            this.formatProvider = formatProvider;
            result = typeInfo.Factory.DynamicInvoke();
            maxValuesNumber = AllValuesNumber;
            if (maxValuesNumber != 0 && LastValue.IsCollection)
                maxValuesNumber += LastValue.Maximum - 1;
        }

        private OptionInfo GetOptionInfo(OptionToken token)
            => typeInfo.GetMatchingOptionInfo(token)
                ?? throw new ArgumentException($"This type does not contain the {token} option.", nameof(token));

        private int AllValuesNumber => typeInfo.Values.Count;
        private ValueInfo LastValue => typeInfo.Values[maxValuesNumber - 1];

        private T GetLastItem<T>(IReadOnlyList<T> list) => list.Count switch
        {
            0 => default,
            int n => list[n - 1]
        };
        
        public IArgumentParser LastAssignedOption => GetLastItem(assignedOptions);
        public IArgumentParser LastAssignedValue => GetLastItem(assignedValues);

        public bool AwaitsValue => usedValuesNumber < maxValuesNumber;

        public void AddOption(OptionToken token)
        {
            var info = GetOptionInfo(token);
            if (info.IsCollection)
                assignedOptions.Add(new CollectionParser(info,
                info.ChooseConverter(convertersFactory) as CollectionConverter, formatProvider));
            else
                assignedOptions.Add(new SingleValueParser(info,
                    info.ChooseConverter(convertersFactory) as IValueConverter, formatProvider));
        }

        public void AddValue(ValueToken token)
        {
            var value = usedValuesNumber < AllValuesNumber ? typeInfo.Values[usedValuesNumber] : LastValue;
            if (!value.IsCollection || LastAssignedValue is CollectionParser)
                assignedValues.Add(new SingleValueParser(value, value.ChooseConverter(convertersFactory) as IValueConverter, formatProvider));
            else
                assignedValues.Add(new CollectionParser(value, value.ChooseConverter(convertersFactory) as CollectionConverter, formatProvider));
            LastAssignedValue.AddValue(token);
            usedValuesNumber++;
        }

        public object Parse()
        {
            assignedOptions.ForEach(o => o.Parse(result));
            assignedValues.ForEach(o => o.Parse(result));
            return result;
        }
    }
}