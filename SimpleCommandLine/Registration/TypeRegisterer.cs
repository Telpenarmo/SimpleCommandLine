using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SimpleCommandLine.Parsing;

namespace SimpleCommandLine.Registration
{
    internal delegate TypeInfo TypeInfoBuilder();

    internal class TypeRegisterer<T> : ITypeRegisterer<T>
    {
        private readonly Func<T> factory;
        private readonly IConvertersFactory convertersFactory;
        private readonly Dictionary<uint, ParameterInfo> values = new();
        private readonly Dictionary<string, ParameterInfo> options = new();
        private IEnumerable<string>? aliases;
        private bool loadFromAttributes = true;

        public TypeRegisterer(Func<T> factory, IConvertersFactory convertersFactory)
        {
            this.factory = factory;
            this.convertersFactory = convertersFactory;
        }

        public TypeInfo Build()
        {
            var finalValues = values;
            var finalOptions = options;
            if (loadFromAttributes)
            {
                var loader = new ReflectionLoader<T>();
                loader.Load();
                if (aliases is null) aliases = loader.Aliases;
                foreach (var item in values)
                    loader.Values[item.Key] = item.Value;
                finalValues = loader.Values;
                foreach (var item in options)
                    loader.Options[item.Key] = item.Value;
                finalOptions = loader.Options;
            }
            finalOptions.Values.ForEach(CheckParameter);
            finalValues.Values.ForEach(CheckParameter);
            var valuesList = finalValues.OrderBy(p => p.Key).Select(p => p.Value).ToArray();
            return aliases != null
                ? new TypeInfo(valuesList, finalOptions, () => factory(), aliases)
                : new TypeInfo(valuesList, finalOptions, () => factory());
        }

        public ITypeRegisterer<T> DiscardAttributes()
        {
            loadFromAttributes = false;
            return this;
        }

        public ITypeRegisterer<T> UseAttributes()
        {
            loadFromAttributes = true;
            return this;
        }

        public ITypeRegisterer<T> RegisterOption<TArg>(Action<T?, TArg?> valueSetter, char shortName, string longName)
        {
            var attribute = new OptionAttribute { LongName = longName, ShortName = shortName };
            return RegisterParameter(attribute, valueSetter, info => SaveOption(attribute, info));
        }

        public ITypeRegisterer<T> RegisterOption<TArg>(Action<T?, TArg?> valueSetter, char shortName)
        {
            var attribute = new OptionAttribute { ShortName = shortName };
            return RegisterParameter(attribute, valueSetter, info => SaveOption(attribute, info));
        }

        public ITypeRegisterer<T> RegisterOption<TArg>(Action<T?, TArg?> valueSetter, string longName)
        {
            var attribute = new OptionAttribute { LongName = longName };
            return RegisterParameter(attribute, valueSetter, info => SaveOption(attribute, info));
        }

        public ITypeRegisterer<T> RegisterValue<TArg>(Action<T?, TArg?> valueSetter, uint index)
        {
            var attribute = new ValueAttribute(index);
            return RegisterParameter(attribute, valueSetter, info => SaveValue(attribute, info));
        }

        private ITypeRegisterer<T> RegisterParameter<TArg>(ParameterAttribute attribute, Action<T?, TArg?> valueSetter, Action<ParameterInfo> saveParameter)
        {
            var info = new ParameterInfo(typeof(TArg), GetSetter(valueSetter), attribute);
            saveParameter(info);
            return this;
        }

        public ITypeRegisterer<T> AsCommand(params string[] aliases)
        {
            this.aliases = aliases;
            return this;
        }

        private void SaveOption(OptionAttribute attribute, ParameterInfo info)
        {
            if (attribute.LongName != null) options[attribute.LongName] = info;
            if (attribute.ShortName != '\0') options[attribute.ShortName.ToString()] = info;
        }

        private void SaveValue(ValueAttribute attribute, ParameterInfo info) => values[attribute.Index] = info;

        private Action<object?, object?> GetSetter<TArg>(Action<T?, TArg?> valueSetter)
        {
            void Setter(object? x, object? y)
            {
                try
                { valueSetter((T?)x, (TArg?)y); }
                catch (InvalidCastException ex)
                { throw new ApplicationException("Internal SimpleCommandLine's error.", ex); }
            }
            return Setter;
        }

        private void CheckParameter(ParameterInfo info)
        {
            var type = info.Type;
            if (!convertersFactory.CheckForType(type))
                throw new InvalidOperationException("No converter registered for:" + type.Name);
        }
    }
}