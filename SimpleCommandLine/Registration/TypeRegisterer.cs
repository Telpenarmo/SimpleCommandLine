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

        public TypeRegisterer(ConvertersFactory convertersFactory) => this.convertersFactory = convertersFactory;

        public TypeInfo Register<T>(Func<T> factory)
        {
            var type = typeof(T);
            var optionPairs = ExtractArgs<OptionAttribute>(type);
            var options = new Dictionary<string, ParameterInfo>();
            foreach (var pair in optionPairs)
            {
                var current = pair.attr.LongName;
                var repeated = current != null && !options.TryAdd(current, pair.info);
                if (!repeated)
                {
                    current = pair.attr.ShortName.ToString();
                    repeated = current != "\0" && !options.TryAdd(current, pair.info);
                }
                if (repeated)
                    throw new InvalidOperationException($"Repeated option: {current}");
            }
            var valuesIndices = new HashSet<uint>();
            var values = ExtractArgs<ValueAttribute>(type)
                .ForEach(p =>
                {
                    if (!valuesIndices.Add(p.attr.Index))
                        throw new InvalidOperationException($"Repeated value index: {p.attr.Index}");
                })
                .OrderBy(p => p.attr.Index).Select(p => p.info).ToArray();

            var info = CreateTypeInfo(type, options, values, factory);
            return info;
        }

        private TypeInfo CreateTypeInfo<T>(Type type,
            IReadOnlyDictionary<string, ParameterInfo> optionInfos,
            IReadOnlyList<ParameterInfo> valueInfos, Func<T> factory)
        {
            var commandAttribute = type.GetTypeInfo().GetCustomAttribute<CommandAttribute>();
            return commandAttribute != null
                ? new TypeInfo(valueInfos, optionInfos, factory, commandAttribute.Aliases)
                : new TypeInfo(valueInfos, optionInfos, factory);
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
            var propertyType = propertyInfo.PropertyType;
            if (!propertyInfo.CanWrite)
                throw new InvalidOperationException("Argument property must have \"set\" accesor.");
            if (!convertersFactory.CheckForType(propertyType))
                throw new InvalidOperationException("No converter registered for:" + propertyType.Name);
        }
    }
}