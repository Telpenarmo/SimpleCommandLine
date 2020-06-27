using System;
using System.Collections.Generic;
using System.Linq;
using SimpleCommandLine.Parsing.Converters;

namespace SimpleCommandLine.Parsing
{
    internal class ConvertersFactory
    {
        private readonly IDictionary<Type, IConverter> converters = new Dictionary<Type, IConverter>();

        public IConverter GetConverter(Type type)
            => converters.ContainsKey(type) || TryFind(type) || TryCreating(type)
                ? converters[type] : null;

        public void RegisterConverter(ISingleValueConverter converter, Type type)
            => converters.Add(type, converter);

        private bool TryFind(Type type)
        {
            var convs = converters.Where(c => type.IsAssignableFrom(c.Key));
            if (!convs.Any()) return false;
            converters[type] = convs.First().Value;
            return true;
        }
        
        private bool TryCreating(Type type)
        {
            if (type.IsCollection())
                return TryCreatingCollectionConverter(type);
            if (type.IsTuple())
                return TryCreatingTupleConverter(type);
            var fallbackConverter = new FallbackValueConverter(type);
            if (!fallbackConverter.CanConvert) return false;
            converters.Add(type, fallbackConverter);
            return true;
        }

        private bool TryCreatingTupleConverter(Type type)
        {
            Type[] typeParams = type.GetTupleElementTypes();
            int valuesNumber = typeParams.Length;
            ISingleValueConverter[] elementConverters = new ISingleValueConverter[valuesNumber];
            for (int i = 0; i < valuesNumber; i++)
            {
                elementConverters[i] = GetConverter(typeParams[i]) as ISingleValueConverter;
            }
            converters[type] = new TupleConverter(elementConverters, type, valuesNumber);
            return true;
        }

        private bool TryCreatingCollectionConverter(Type type)
        {
            Type elementType = type.GetCollectionElementType();
            if (!(converters.ContainsKey(elementType)
                && converters[elementType] is ISingleValueConverter valueConverter))
                return false; // failure when element's type is not convertable

            if (type.IsArray || type.IsAssignableFrom(typeof(Array)))
            {
                converters[type] = new ArrayConverter(elementType, valueConverter);
                return true;
            }

            else if (!type.IsGenericType) return false;
            var typeDef = type.GetGenericTypeDefinition();

            if (typeof(IList<>) == typeDef
                || typeof(IEnumerable<>) == typeDef
                || typeof(ICollection<>) == typeDef
                || typeof(IReadOnlyCollection<>) == typeDef
                || typeof(IReadOnlyList<>) == typeDef)
            {
                converters[type] = new GenericCollectionConverter(typeof(List<>)
                    .MakeGenericType(elementType), valueConverter);
                return true;
            }

            if (typeof(LinkedList<>) == typeDef
                || typeof(Queue<>) == typeDef
                || typeof(Stack<>) == typeDef
                || typeof(List<>) == typeDef
                || typeof(HashSet<>) == typeDef
                || typeof(SortedSet<>) == typeDef)
            {
                converters[type] = new GenericCollectionConverter(type, valueConverter);
                return true;
            }

            var enumerableDef = typeof(IEnumerable<>).MakeGenericType(elementType);
            var constructor = type.GetConstructor(new[] { enumerableDef });
            if (constructor == null) return false;
            converters[type] = new GenericCollectionConverter(type, valueConverter);
            return true;
        }
    }
}