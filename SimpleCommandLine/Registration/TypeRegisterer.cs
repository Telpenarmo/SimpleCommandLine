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
        private readonly ConvertersFactory convertersFactory;
        private readonly Dictionary<uint, ParameterInfo> values = new Dictionary<uint, ParameterInfo>();
        private readonly Dictionary<string, ParameterInfo> options = new Dictionary<string, ParameterInfo>();
        private IEnumerable<string>? aliases;
        private bool loadFromAttributes = true;

        public TypeRegisterer(Func<T> factory, ConvertersFactory convertersFactory)
        {
            this.factory = factory;
            this.convertersFactory = convertersFactory;
        }

        public TypeInfo Build()
        {
            if (loadFromAttributes) LoadWithReflection();
            var valuesList = values.OrderBy(p => p.Key).Select(p => p.Value).ToArray();
            return aliases != null
                ? new TypeInfo(valuesList, options, () => factory(), aliases)
                : new TypeInfo(valuesList, options, () => factory());
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
            var current = attribute.LongName;
            var repeated = current != null && !options.TryAdd(current, info);
            if (!repeated)
            {
                current = attribute.ShortName.ToString();
                repeated = current != "\0" && !options.TryAdd(current, info);
            }
            else throw new InvalidOperationException($"Repeated option: {current}");
        }

        private void SaveValue(ValueAttribute attribute, ParameterInfo info)
        {
            if (!values.TryAdd(attribute.Index, info))
                throw new InvalidOperationException($"Repeated value index: {attribute.Index}");
        }

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

        private void LoadWithReflection()
        {
            var type = typeof(T);
            ExtractArgs<OptionAttribute>(type).ForEach(pair => SaveOption(pair.attr, pair.info));
            ExtractArgs<ValueAttribute>(type).ForEach(p => SaveValue(p.attr, p.info));

            var commandAttribute = type.GetTypeInfo().GetCustomAttribute<CommandAttribute>();
            if (aliases == null && commandAttribute != null)
                aliases = commandAttribute.Aliases;
        }

        private IEnumerable<(TAttr attr, ParameterInfo info)> ExtractArgs<TAttr>(Type type)
            where TAttr : ParameterAttribute
            => type.GetProperties()
                .Select(property => (property, attr: property.GetCustomAttribute<TAttr>()))
                .Where(pair => pair.attr != null)
                .ForEach(pair => CheckProperty(pair.property))
                .Select(p => (p.attr, new ParameterInfo(p.property.PropertyType, p.property.SetValue, p.attr)));

        private void CheckProperty(PropertyInfo propertyInfo)
        {
            if (!propertyInfo.CanWrite)
                throw new InvalidOperationException("Argument property must have \"set\" accesor.");
            var propertyType = propertyInfo.PropertyType;
            if (!convertersFactory.CheckForType(propertyType))
                throw new InvalidOperationException("No converter registered for:" + propertyType.Name);
        }
    }
}