using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SimpleCommandLine.Registration.Validation;

namespace SimpleCommandLine.Registration
{
    internal class TypeRegisterer : ITypeRegisterer
    {
        private readonly ITypeValidator typeValidator;
        private readonly IPropertyValidator propertyValidator;
        private readonly IList<OptionAttribute> optionAttributes = new List<OptionAttribute>();

        public TypeRegisterer(ITypeValidator typeValidator, IPropertyValidator propertyValidator)
        {
            this.typeValidator = typeValidator ?? throw new ArgumentNullException(nameof(typeValidator));
            this.propertyValidator = propertyValidator ?? throw new ArgumentNullException(nameof(propertyValidator));
        }

        public ParsingTypeInfo Register<T>(Func<T> factory)
        {
            var type = typeof(T);
            var options = ExtractArgs<OptionAttribute, ParsingOptionInfo>(type,
                pair => new ParsingOptionInfo(pair.Item1, pair.Item2),
                attr => optionAttributes.Add(attr));
            var values = ExtractArgs<ValueAttribute, ParsingValueInfo>(type,
                pair => new ParsingValueInfo(pair.Item1, pair.Item2));
            ParsingTypeInfo typeInfo = CreateTypeInfo(type, options, values, factory);
            typeValidator.Verify(typeInfo, optionAttributes);
            return typeInfo;
        }

        private ParsingTypeInfo CreateTypeInfo<T>(Type type, IEnumerable<ParsingOptionInfo> optionInfos, IEnumerable<ParsingValueInfo> valueInfos, Func<T> factory)
        {
            ParsingTypeInfo typeInfo;
            var commandAttribute = type.GetTypeInfo().GetCustomAttribute<CommandAttribute>();
            if (commandAttribute != null)
                typeInfo = new ParsingCommandTypeInfo(valueInfos, optionInfos, factory, commandAttribute);
            else
                typeInfo = new ParsingTypeInfo(valueInfos, optionInfos, factory);
            return typeInfo;
        }

        private IEnumerable<TInfo> ExtractArgs<TAttr, TInfo>(Type type, Func<(PropertyInfo, TAttr), TInfo> factory, Action<TAttr> action = null)
            where TAttr : ArgumentAttribute where TInfo : ParsingArgumentInfo
            => type.GetProperties()
                .Select(property => (property, property.GetCustomAttribute<TAttr>()))
                .Where(pair => pair.Item2 != null)
                .ForEach(pair => action?.Invoke(pair.Item2))
                .Where(pair => propertyValidator.Verify(pair.property))
                .Select(pair => factory(pair))
                .ToArray();
    }
}