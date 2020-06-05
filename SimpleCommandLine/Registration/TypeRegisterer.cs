using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SimpleCommandLine.Parsing;
using SimpleCommandLine.Registration.Validation;

namespace SimpleCommandLine.Registration
{
    internal class TypeRegisterer
    {
        private readonly TypeValidator typeValidator;
        private readonly IConvertersFactory convertersFactory;
        private readonly IList<OptionAttribute> optionAttributes = new List<OptionAttribute>();

        public TypeRegisterer(TypeValidator typeValidator, IConvertersFactory convertersFactory)
        {
            this.typeValidator = typeValidator;
            this.convertersFactory = convertersFactory;
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

        private ParsingTypeInfo CreateTypeInfo<T>(Type type,
            IEnumerable<ParsingOptionInfo> optionInfos,
            IEnumerable<ParsingValueInfo> valueInfos, Func<T> factory)
        {
            ParsingTypeInfo typeInfo;
            var commandAttribute = type.GetTypeInfo().GetCustomAttribute<CommandAttribute>();
            if (commandAttribute != null)
                typeInfo = new ParsingTypeInfo(valueInfos, optionInfos, factory, commandAttribute.Name);
            else
                typeInfo = new ParsingTypeInfo(valueInfos, optionInfos, factory);
            return typeInfo;
        }

        private IEnumerable<TInfo> ExtractArgs<TAttr, TInfo>(Type type,
            Func<(PropertyInfo, TAttr), TInfo> factory,
            Action<TAttr> action = null)
            where TAttr : ArgumentAttribute where TInfo : ParsingArgumentInfo
            => type.GetProperties()
                .Select(property => (property, property.GetCustomAttribute<TAttr>()))
                .Where(pair => pair.Item2 != null)
                .ForEach(pair => action?.Invoke(pair.Item2))
                .Where(pair => CheckProperty(pair.property))
                .Select(pair => factory(pair))
                .ToArray();

        public bool CheckProperty(PropertyInfo propertyInfo)
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
            return true;
        }
    }
}