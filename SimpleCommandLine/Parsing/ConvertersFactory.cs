using System;
using System.Collections.Generic;
using SimpleCommandLine.Parsing.Converters;

namespace SimpleCommandLine.Parsing
{
    internal class ConvertersFactory
    {
        private readonly IDictionary<Type, IConverter> valueConverters = new Dictionary<Type, IConverter>();

        public IConverter GetConverter(Type type)
            => valueConverters.ContainsKey(type) || TryCreating(type) ? valueConverters[type] : null;

        public void RegisterConverter(IValueConverter converter, Type type)
        {
            if (converter == null)
                throw new ArgumentNullException(nameof(converter));
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            valueConverters.Add(type, converter);
        }

        private bool TryCreating(Type type)
        {
            if (type.IsCollection())
                return TryCreatingCollectionConverter(type);
            var fallbackConverter = new FallbackValueConverter(type);
            if (fallbackConverter.CanConvert)
                valueConverters.Add(type, fallbackConverter);

            return valueConverters.ContainsKey(type);
        }

        private bool TryCreatingCollectionConverter(Type type)
        {
            Type elementType = type.GetCollectionElementType();
            if (!(valueConverters.ContainsKey(elementType) && valueConverters[elementType] is IValueConverter valueConverter))
                return false; // failure when element's type is not convertable
            if (type.IsArray)
            {
                valueConverters[type] = new ArrayConverter(elementType, valueConverter);
                return true;
            }
            else if (!type.IsGenericType) return false;

            var typeDef = type.GetGenericTypeDefinition();

            if (typeof(IList<>) == typeDef
                || typeof(IEnumerable<>) == typeDef
                || typeof(ICollection<>) == typeDef
                || typeof(IReadOnlyCollection<>) == typeDef
                || typeof(IReadOnlyList<>) == typeDef
                || typeof(List<>) == typeDef

                || typeof(LinkedList<>) == typeDef
                || typeof(Queue<>) == typeDef
                || typeof(Stack<>) == typeDef

                || typeof(HashSet<>) == typeDef
                || typeof(ISet<>) == typeDef)
            {
                valueConverters[type] = new GenericCollectionConverter(type, valueConverter);
                return true;
            }

            var enumerableDef = typeof(IEnumerable<>).MakeGenericType(elementType);
            var constructor = type.GetConstructor(new[] { enumerableDef });
            if (constructor != null)
            {
                valueConverters[type] = new GenericCollectionConverter(type, valueConverter);
                return true;
            }

            return false;
        }
    }
}