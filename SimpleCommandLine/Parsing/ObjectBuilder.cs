using System;
using System.Collections.Generic;
using System.Linq;
using SimpleCommandLine.Registration;
using SimpleCommandLine.Tokens;

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
        private int usedValuesNumber = 0;

        public ObjectBuilder(TypeInfo typeInfo, ConvertersFactory convertersFactory, IFormatProvider formatProvider)
        {
            this.typeInfo = typeInfo;
            this.convertersFactory = convertersFactory;
            this.formatProvider = formatProvider;
            result = typeInfo.Factory.DynamicInvoke();
        }

        private int AllValuesNumber => typeInfo.Values.Count;

        private T Last<T>(IReadOnlyList<T> list) => list.Count switch
        {
            0 => default,
            int n => list[n - 1]
        };

        public IArgumentParser LastAssignedOption => Last(assignedOptions);
        private IArgumentParser LastAssignedValue => Last(assignedValues);

        public bool AwaitsValue => usedValuesNumber < AllValuesNumber || (LastAssignedValue?.AcceptsValue ?? false);

        public bool TryAddOption(OptionToken token)
        {
            var info = typeInfo.GetMatchingOptionInfo(token);
            if (info is null) return false;
            else if (info.IsMulltiValued)
                assignedOptions.Add(new CollectionParser(info,
                info.ChooseConverter(convertersFactory) as CollectionConverter, formatProvider));
            else
                assignedOptions.Add(new SingleValueParser(info,
                    info.ChooseConverter(convertersFactory) as ISingleValueConverter, formatProvider));
            return true;
        }

        public void AddValue(ValueToken token)
        {
            if (!(LastAssignedValue?.AcceptsValue ?? false))
            {
                var next = typeInfo.Values[usedValuesNumber];
                if (next.IsMulltiValued)
                    assignedValues.Add(new CollectionParser(next,
                        next.ChooseConverter(convertersFactory) as IMultipleValueConverter, formatProvider));
                else
                    assignedValues.Add(new SingleValueParser(next,
                        next.ChooseConverter(convertersFactory) as ISingleValueConverter, formatProvider));
                usedValuesNumber++;
            }
            LastAssignedValue.AddValue(token);
        }

        public ParsingResult Parse()
        {
            foreach (var o in assignedOptions.Concat(assignedValues))
            {
                var r = o.Parse(result);
                if (r?.IsError ?? false) return r;
            }
            return ParsingResult.Success(result);
        }
    }
}