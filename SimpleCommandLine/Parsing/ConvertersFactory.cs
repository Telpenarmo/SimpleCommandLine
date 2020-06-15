using System;
using System.Collections.Generic;
using SimpleCommandLine.Parsing.Converters;

namespace SimpleCommandLine.Parsing
{
    internal class ConvertersFactory : IConvertersFactory
    {
        private readonly IDictionary<Type, IConverter> valueConverters = new Dictionary<Type, IConverter>();

        public IConverter<object> GetConverter(Type type)
            => valueConverters.ContainsKey(type) || TryBuild(type) ? valueConverters[type] : null;

        public void RegisterConverter(IValueConverter converter, Type type)
        {
            if (converter == null)
                throw new ArgumentNullException(nameof(converter));
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            valueConverters.Add(type, converter as IConverter);
        }

        private bool TryBuild(Type type)
        {
            Type elementType = type.GetCollectionElementType();
            if (!(valueConverters.ContainsKey(type) && valueConverters[type] is IValueConverter valueConverter))
                return false;

            CollectionConverter result = null;
            if (type.IsArray)
                result = new ArrayConverter(elementType, valueConverter);

            else if (!type.IsGenericType)
                return false;

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
                result = new GenericCollectionConverter(type, valueConverter);

            var enumerableDef = typeof(IEnumerable<>).MakeGenericType(elementType);
            var constructor = type.GetConstructor(new[] { enumerableDef });
            if (constructor != null)
                result = new GenericCollectionConverter(type, valueConverter);

            valueConverters[type] = result as IConverter;
            return true;
        }
    }
}