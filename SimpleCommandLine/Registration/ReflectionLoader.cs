using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SimpleCommandLine.Registration
{
    internal class ReflectionLoader<T>
    {
        public void Load()
        {
            var type = typeof(T);
            ExtractArgs<OptionAttribute>(type).ForEach(p => AddOption(p.attr, p.info));
            ExtractArgs<ValueAttribute>(type).ForEach(p => AddValue(p.attr, p.info));

            var commandAttribute = type.GetTypeInfo().GetCustomAttribute<CommandAttribute>();
            if (commandAttribute != null)
                Aliases = commandAttribute.Aliases;
        }

        public Dictionary<string, ParameterInfo> Options { get; private set; } = new();
        public Dictionary<uint, ParameterInfo> Values { get; private set; } = new();
        public IEnumerable<string>? Aliases { get; private set; }

        private void AddOption(OptionAttribute attribute, ParameterInfo info)
        {
            var current = attribute.LongName;
            var repeated = current != null && !Options.TryAdd(current, info);
            if (!repeated)
            {
                current = attribute.ShortName.ToString();
                repeated = current != "\0" && !Options.TryAdd(current, info);
            }
            if (repeated) throw new InvalidOperationException($"Repeated option: {current}");
        }

        private void AddValue(ValueAttribute attribute, ParameterInfo info)
        {
            if (!Values.TryAdd(attribute.Index, info))
                throw new InvalidOperationException($"Repeated value index: {attribute.Index}");
        }

        private IEnumerable<(TAttr attr, ParameterInfo info)> ExtractArgs<TAttr>(Type type)
            where TAttr : ParameterAttribute
        {
            var pairs = type.GetProperties()
                            .Select(property => (property, attr: property.GetCustomAttribute<TAttr>()))
                            .Where(pair => pair.attr != null);
            pairs.ForEach(pair => CheckProperty(pair.property));
            return pairs.Select(p => (p.attr, new ParameterInfo(p.property.PropertyType, p.property.SetValue, p.attr)));
        }

        private void CheckProperty(PropertyInfo propertyInfo)
        {
            if (!propertyInfo.CanWrite)
                throw new InvalidOperationException("Argument property must have \"set\" accesor.");
        }
    }
}