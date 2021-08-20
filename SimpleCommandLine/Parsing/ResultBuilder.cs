using System;
using System.Collections.Generic;
using System.Linq;
using SimpleCommandLine.Registration;
using SimpleCommandLine.Tokens;

namespace SimpleCommandLine.Parsing
{
    internal sealed class ResultBuilder
    {
        private readonly TypeInfo typeInfo;
        private readonly ConvertersFactory converters;
        private readonly List<IArgumentHandler> assignedOptions = new();
        private readonly List<IArgumentHandler> assignedValues = new();
        private readonly List<string> errors = new();
        private readonly object? result;
        private readonly IFormatProvider formatProvider;
        private int usedValuesNumber = 0;
        private bool unknownOption = false;

        public ResultBuilder(TypeInfo typeInfo, ConvertersFactory converters, IFormatProvider formatProvider)
        {
            this.typeInfo = typeInfo;
            this.converters = converters;
            this.formatProvider = formatProvider;
            result = typeInfo.Factory();
        }

        private bool CollectionValueAwaits => assignedValues.LastOrDefault()?.AcceptsValue ?? false;
        private bool AwaitsValue => usedValuesNumber < typeInfo.Values.Count || CollectionValueAwaits;

        private void AddNewParser(List<IArgumentHandler> parsers, ParameterInfo info)
        {
            IArgumentHandler newParser;
            if (info.Type.IsCollection() || info.Type.IsTuple())
                newParser = new CollectionHandler(info, converters[info.Type], formatProvider);
            else
                newParser = new UnaryArgumentHandler(info, converters[info.Type], formatProvider);
            parsers.Add(newParser);
        }

        public void HandleToken(IArgumentToken token)
        {
            switch (token)
            {
                case OptionToken option:
                    HandleOption(option);
                    break;
                case ValueToken value:
                    HandleValue(value);
                    break;
                case OptionsGroupToken group:
                    group.Tokens.ForEach(o => HandleOption(o));
                    break;
                case AssignedValueToken assignedValue:
                    HandleOption(assignedValue.Option);
                    if (!unknownOption)
                        assignedOptions.LastOrDefault()?.SetValue(assignedValue.Value);
                    break;
            }
        }

        private void HandleOption(OptionToken token)
        {
            EnsureLastOptionSet();
            var info = typeInfo.GetMatchingOptionInfo(token);
            unknownOption = info is null;
            if (unknownOption)
                errors.Add($"The current type does not contain the \"{token}\" option.");
            else
#nullable disable // we did this check!
                AddNewParser(assignedOptions, info);
#nullable enable
        }

        private void HandleValue(ValueToken token)
        {
            if (unknownOption) return;
            if (assignedOptions.LastOrDefault()?.AcceptsValue ?? false)
                assignedOptions.Last().AddValue(token);
            else if (AwaitsValue)
            {
                if (!CollectionValueAwaits)
                {
                    var next = typeInfo.Values[usedValuesNumber];
                    AddNewParser(assignedValues, next);
                    usedValuesNumber++;
                }
                assignedValues.Last().AddValue(token);
            }
            else
                errors.Add("The current type does not accept any more values.");
        }

        public void AddValue(ValueToken token)
        {
            if (!CollectionValueAwaits)
            {
                var next = typeInfo.Values[usedValuesNumber];
                AddNewParser(assignedValues, next);
                usedValuesNumber++;
            }
            assignedValues.Last().AddValue(token);
        }

        public ParsingResult Build()
        {
            var applications = new List<Action>();
            foreach (var handler in assignedOptions.Concat(assignedValues))
            {
                var argResult = handler.GetResult();
                if (argResult.ErrorMessages != null)
                    errors.AddRange(argResult.ErrorMessages);
                else if (errors.IsEmpty())
                    applications.Add(() => handler.ParameterInfo.SetValue(result, argResult.ResultObject));
            }

            if (errors.Any()) return ParsingResult.Error(errors);
            applications.ForEach(app => app());
            return ParsingResult.Success(result);
        }

        private void EnsureLastOptionSet()
        {
            if (assignedValues.LastOrDefault()?.RequiresValue ?? false)
                errors.Add("Value was not provided for an option.");
        }
    }
}