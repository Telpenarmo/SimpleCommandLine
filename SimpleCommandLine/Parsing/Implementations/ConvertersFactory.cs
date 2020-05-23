using System;
using System.Collections.Generic;
using SimpleCommandLine.Parsing.Converters;

namespace SimpleCommandLine.Parsing
{
    internal class ConvertersFactory : IConvertersFactory
    {
        private readonly IDictionary<Type, IValueConverter> valueConverters = new Dictionary<Type, IValueConverter>();
        private readonly CollectionConvertersFactory collectionConvertersFactory;

        public ConvertersFactory()
        {
            collectionConvertersFactory = new CollectionConvertersFactory(this);
        }

        public IConverter GetConverter(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            if (type.IsCollection())
                return collectionConvertersFactory.GetConverter(type);
            else
                return valueConverters.ContainsKey(type) || TryBuild(type) ? valueConverters[type] : null;
        }

        public void RegisterConverter(IValueConverter converter, Type type)
        {
            if (converter == null)
                throw new ArgumentNullException(nameof(converter));
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            valueConverters.Add(type, converter);
        }

        private bool TryBuild(Type type)
        {
            if (type.IsEnum)
                valueConverters.Add(type, new EnumConverter(type));
            else
            {
                var fallbackConverter = new FallbackValueConverter(type);
                if (fallbackConverter.CanConvert)
                    valueConverters.Add(type, fallbackConverter);
            }
            return valueConverters.ContainsKey(type);
        }
    }
}
