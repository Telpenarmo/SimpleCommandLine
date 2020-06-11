using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SimpleCommandLine.Parsing;

namespace SimpleCommandLine.Registration
{
    internal class TypeRegisterer
    {
        private readonly IConvertersFactory convertersFactory;
        private readonly HashSet<string> LongOptions = new HashSet<string>();
        private readonly HashSet<string> ShortOptions = new HashSet<string>();
        private readonly HashSet<uint> ValuesIndices = new HashSet<uint>();

        public TypeRegisterer(IConvertersFactory convertersFactory)
        {
            this.convertersFactory = convertersFactory;
        }

        public ParsingTypeInfo Register<T>(Func<T> factory)
        {
            var type = typeof(T);
            var options = ExtractArgs<OptionAttribute, ParsingOptionInfo>(type,
                pair => new ParsingOptionInfo(pair.Item1, pair.Item2), CheckOption);
            var values = ExtractArgs<ValueAttribute, ParsingValueInfo>(type,
                pair => new ParsingValueInfo(pair.Item1, pair.Item2), CheckValue);

            return CreateTypeInfo(type, options, values, factory);
        }

        private ParsingTypeInfo CreateTypeInfo<T>(Type type,
            IEnumerable<ParsingOptionInfo> optionInfos,
            IEnumerable<ParsingValueInfo> valueInfos, Func<T> factory)
        {
            var commandAttribute = type.GetTypeInfo().GetCustomAttribute<CommandAttribute>();
            return commandAttribute != null
                ? new ParsingTypeInfo(valueInfos, optionInfos, factory, commandAttribute.Name)
                : new ParsingTypeInfo(valueInfos, optionInfos, factory);
        }

        private IEnumerable<TInfo> ExtractArgs<TAttr, TInfo>(Type type,
            Func<(PropertyInfo, TAttr), TInfo> factory,
            Action<TAttr> customCheck = null)
            where TAttr : ArgumentAttribute where TInfo : ParsingArgumentInfo
            => type.GetProperties()
                .Select(property => (property, attr: property.GetCustomAttribute<TAttr>()))
                .Where(pair => pair.attr != null)
                .ForEach(pair =>
                {
                    CheckProperty(pair.property);
                    customCheck?.Invoke(pair.attr);
                })
                .Select(pair => factory(pair));

        private void CheckProperty(PropertyInfo propertyInfo)
        {
            var propertyType = propertyInfo.PropertyType;
            if (!propertyInfo.CanWrite)
                throw new InvalidOperationException("Argument property must have \"set\" accesor.");
            if (propertyType.IsAbstract)
                throw new InvalidOperationException("Type of the property must not be abstract.");
            if (propertyType.GetCollectionElementType()?.IsCollection() ?? false)
                throw new NotSupportedException("Nested collections are not supported.");
            if (propertyType.GetCustomAttribute<FlagsAttribute>() != null)
                throw new NotSupportedException("Flag enumerations are not supported.");
            if (convertersFactory.GetConverter(propertyType) is null)
                throw new InvalidOperationException("No converter registered for a type.");
        }

        private void CheckOption(OptionAttribute attr)
        {
            if (!(LongOptions.Add(attr.LongName) && ShortOptions.Add(attr.ShortName)))
                throw new InvalidOperationException("Options must have different names.");
        }

        private void CheckValue(ValueAttribute attr)
        {
            if (!ValuesIndices.Add(attr.Index))
                throw new InvalidOperationException("Values must have different indices.");
        }
    }
}