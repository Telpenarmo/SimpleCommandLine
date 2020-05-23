using System;
using System.Collections.Generic;
using SimpleCommandLine.Parsing.Converters;

namespace SimpleCommandLine.Parsing
{
    internal class CollectionConvertersFactory
    {
        private readonly IConvertersFactory convertersFactory;

        public CollectionConvertersFactory(IConvertersFactory convertersFactory)
        {
            this.convertersFactory = convertersFactory ?? throw new ArgumentNullException(nameof(convertersFactory));
        }

        public CollectionConverter GetConverter(Type type)
        {
            Type elementType = type.GetCollectionElementType();
            if (!(convertersFactory.GetConverter(elementType) is IValueConverter valueConverter))
                return null;

            if (type.IsArray)
                return new ArrayConverter(elementType, valueConverter);

            if (type.IsGenericType)
            {
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
