using System;
using System.Collections.Generic;
using SimpleCommandLine.Registration;
using SimpleCommandLine.Parsing.Converters;

namespace SimpleCommandLine.Parsing
{
    internal class ValueConvertersFactory : IValueConvertersFactory
    {
        private readonly IDictionary<Type, IValueConverter> valueConverters = new Dictionary<Type, IValueConverter>();

        public IValueConverter GetConverter(ParsingArgumentInfo argumentInfo)
        {
            if (argumentInfo == null) throw new ArgumentNullException(nameof(argumentInfo));

            var type = argumentInfo.PropertyType;
            if (valueConverters.ContainsKey(type) || Fallback(type))
                return valueConverters[type];
            else
                throw new ArgumentException();
        }

        public bool CanConvert(Type type)
        {
            if (type is null)
                return false;

            if (valueConverters.ContainsKey(type))
                return true;
            else
                return Fallback(type);
        }

        public void Register(IValueConverter converter, Type type)
        {
            valueConverters.Add(type, converter);
        }

        private bool Fallback(Type type)
        {
            var fallbackConverter = new FallbackValueConverter(type);
            if (fallbackConverter.CanConvert)
            {
                valueConverters.Add(type, fallbackConverter);
                return true;
            }
            else
                return false;
        }
    }
}
