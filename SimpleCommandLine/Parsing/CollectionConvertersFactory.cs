using System;
using System.Collections.Generic;
using System.Linq;
using SimpleCommandLine.Parsing.Converters;

namespace SimpleCommandLine.Parsing
{
    internal class CollectionConvertersFactory : ICollectionConvertersFactory
    {
        private readonly IConvertersFactory convertersFactory;

        public CollectionConvertersFactory(IConvertersFactory convertersFactory)
        {
            this.convertersFactory = convertersFactory ?? throw new ArgumentNullException(nameof(convertersFactory));
        }

        public CollectionConverter GetConverter(Type type)
        {
            Type elementType;
            IValueConverter valueConverter;
            if (type.IsArray)
            {
                elementType = type.GetElementType();
                valueConverter = convertersFactory.GetConverter(elementType) as IValueConverter;
                return new ArrayConverter(elementType, valueConverter);
            }

            if (type.IsGenericType)
            {
                var typeDef = type.GetGenericTypeDefinition();
                elementType = type.GetGenericArguments().First();
                valueConverter = convertersFactory.GetConverter(elementType) as IValueConverter;

                if (typeof(IList<>) == typeDef
                    || typeof(IEnumerable<>) == typeDef
                    || typeof(ICollection<>) == typeDef
                    || typeof(IReadOnlyCollection<>) == typeDef
                    || typeof(IReadOnlyList<>) == typeDef
                    || typeof(List<>) == typeDef)
                    return new GenericCollectionConverter(type, valueConverter);

                if (typeof(ISet<>) == typeDef
                  || typeof(HashSet<>) == typeDef)
                    return new GenericCollectionConverter(type, valueConverter);

                if (typeof(Queue<>) == typeDef)
                    return new GenericCollectionConverter(type, valueConverter);
                if (typeof(Stack<>) == typeDef)
                    return new GenericCollectionConverter(type, valueConverter);
                if (typeof(LinkedList<>) == typeDef)
                    return new GenericCollectionConverter(type, valueConverter);

                var enumerableDef = typeof(IEnumerable<>).MakeGenericType(elementType);
                var constructor = type.GetConstructor(new[] { enumerableDef });
                if (constructor != null)
                    return new GenericCollectionConverter(type, valueConverter);
            }

            return null;
        }
    }
}
