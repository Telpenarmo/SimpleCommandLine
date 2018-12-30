using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SimpleCommandLine.Registration.Validation;

namespace SimpleCommandLine.Registration
{
    internal class TypeRegisterer : ITypeRegisterer
    {
        readonly ITypeValidator typeValidator;
        readonly IPropertyValidator propertyValidator;
        readonly IList<OptionAttribute> optionAttributes = new List<OptionAttribute>();

        public TypeRegisterer(ITypeValidator typeValidator, IPropertyValidator propertyValidator)
        {
            this.typeValidator = typeValidator ?? throw new ArgumentNullException(nameof(typeValidator));
            this.propertyValidator = propertyValidator ?? throw new ArgumentNullException(nameof(propertyValidator));
        }

        public ParsingTypeInfo Register<T>(Func<T> factory)
        {
            var type = typeof(T);
            var options = ExtractOptions(type);
            var values = ExtractValues(type);
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

        private IEnumerable<ParsingOptionInfo> ExtractOptions(Type type)
        {
            OptionAttribute attribute = null;
            return type.GetProperties()
                .Where(property =>
                {
                    attribute = property.GetCustomAttribute<OptionAttribute>();
                    if (attribute != null)
                    {
                        optionAttributes.Add(attribute);
                        return true;
                    }
                    else
                        return false;
                })
                .Where(property => propertyValidator.Verify(property))
                .Select(property => new ParsingOptionInfo(property, attribute))
                .ToArray();
        }

        private IEnumerable<ParsingValueInfo> ExtractValues(Type type)
        {
            ValueAttribute attribute = null;
            return type.GetProperties()
                .Where(property =>
                {
                    attribute = property.GetCustomAttribute<ValueAttribute>();
                    return attribute != null;
                })
                .Where(property => propertyValidator.Verify(property))
                .Select(property => new ParsingValueInfo(property, attribute))
                .ToArray();
        }
    }
}