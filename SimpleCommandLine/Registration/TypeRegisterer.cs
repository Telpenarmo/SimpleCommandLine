using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SimpleCommandLine.Parsing;

namespace SimpleCommandLine.Registration
{
    internal class TypeRegisterer
    {
        private readonly ConvertersFactory convertersFactory;
        private readonly HashSet<string> longOptions = new HashSet<string>();
        private readonly HashSet<string> shortOptions = new HashSet<string>();
        private readonly HashSet<uint> valuesIndices = new HashSet<uint>();

        public TypeRegisterer(ConvertersFactory convertersFactory)
        {
            this.convertersFactory = convertersFactory;
        }

        public TypeInfo Register<T>(Func<T> factory)
        {
            var type = typeof(T);
            var options = ExtractArgs<OptionAttribute, OptionInfo>(type,
                pair => new OptionInfo(pair.Item1, pair.Item2), CheckOption);
            var values = ExtractArgs<ValueAttribute, ValueInfo>(type,
                pair => new ValueInfo(pair.Item1, pair.Item2), CheckValue);

            return CreateTypeInfo(type, options, values, factory);
        }

        private TypeInfo CreateTypeInfo<T>(Type type,
            IEnumerable<OptionInfo> optionInfos,
            IEnumerable<ValueInfo> valueInfos, Func<T> factory)
        {
            var commandAttribute = type.GetTypeInfo().GetCustomAttribute<CommandAttribute>();
            return commandAttribute != null
                ? new TypeInfo(valueInfos, optionInfos, factory, commandAttribute.Name)
                : new TypeInfo(valueInfos, optionInfos, factory);
        }

        private IEnumerable<TInfo> ExtractArgs<TAttr, TInfo>(Type type,
            Func<(PropertyInfo, TAttr), TInfo> factory,
            Action<TAttr> customCheck = null)
            where TAttr : ArgumentAttribute where TInfo : ArgumentInfo
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
            if (propertyType.IsCollection() && (propertyType.GetCollectionElementType()?.IsCollection() ?? false))
                throw new NotSupportedException("Nested collections are not supported.");
            if (propertyType.GetCustomAttribute<FlagsAttribute>() != null)
                throw new NotSupportedException("Flag enumerations are not supported.");
            if (convertersFactory.GetConverter(propertyType) is null)
                throw new InvalidOperationException("No converter registered for:" + propertyType.Name);
        }

        private void CheckOption(OptionAttribute attr)
        {
            if (!(AddIfNotNull(longOptions, attr.LongName)
                && AddIfNotNull(shortOptions, attr.ShortName)))
                throw new InvalidOperationException("Options must have different names.");
        }

        private void CheckValue(ValueAttribute attr)
        {
            if (!valuesIndices.Add(attr.Index))
                throw new InvalidOperationException("Values must have different indices.");
        }

        private bool AddIfNotNull(HashSet<string> set, string item) => item == null || set.Add(item);
    }
}