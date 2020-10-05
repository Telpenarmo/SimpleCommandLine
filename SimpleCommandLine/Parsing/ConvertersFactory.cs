using System;
using System.Collections.Generic;
using System.Linq;
using SimpleCommandLine.Parsing.Converters;

namespace SimpleCommandLine.Parsing
{
    internal class ConvertersFactory
    {
        private readonly IDictionary<Type, IConverter> converters = new Dictionary<Type, IConverter>();
        public ParsingSettings Settings { get; set; }

        public IConverter this[Type type] => converters[type];

        public bool CheckForType(Type type)
            => converters.ContainsKey(type) || TryCreating(type) || TryFallback(type);

        public void RegisterConverter(ISingleValueConverter converter, Type type)
            => converters.Add(type, converter);

        private bool TryCreating(Type type)
        {
            if (type.IsCollection())
                return TryCreatingCollectionConverter(type);
            if (type.IsTuple())
                return TryCreatingTupleConverter(type);
            if (type.IsEnum)
            {
                converters[type] = new EnumConverter(type,
                    Settings.IgnoreCaseOnEnumConversion,
                    Settings.AcceptNumericalEnumValues);
                return true;
            }
            var fallback = System.ComponentModel.TypeDescriptor.GetConverter(type);
            if (fallback.CanConvertFrom(typeof(string)))
                converters[type] = new TypeDescriptorConverter(fallback);

            return true;
        }

        private bool TryCreatingTupleConverter(Type type)
        {
            Type[] typeParams = type.GetTupleElementTypes();
            int valuesNumber = typeParams.Length;
            IConverter[] elementConverters = new IConverter[valuesNumber];
            for (int i = 0; i < valuesNumber; i++)
            {
                if (CheckForType(typeParams[i]))
                    elementConverters[i] = this[typeParams[i]];
                else return false;
            }

            converters[type] = new TupleConverter(type, valuesNumber, elementConverters);
            return true;
        }

        private bool TryCreatingCollectionConverter(Type type)
        {
            if (type.IsEnum)
            {
                converters[type] = new FlagsEnumConverter(type,
                    Settings.IgnoreCaseOnEnumConversion,
                    Settings.AcceptNumericalEnumValues);
                return true;
            }

            Type elementType = type.GetCollectionElementType();
            if (!CheckForType(elementType))
                return false; // failure when element's type is not convertable
            var elementConverter = this[elementType];

            if (type.IsArray || type.IsAssignableFrom(typeof(Array)))
            {
                converters[type] = new ArrayConverter(elementType, elementConverter);
                return true;
            }

            else if (!type.IsGenericType) return false;
            var typeDef = type.GetGenericTypeDefinition();

            if (type.IsInterface &&
               (typeof(IList<>) == typeDef
               || typeof(IEnumerable<>) == typeDef
               || typeof(ICollection<>) == typeDef
               || typeof(IReadOnlyCollection<>) == typeDef
               || typeof(IReadOnlyList<>) == typeDef))
            {
                converters[type] = new GenericCollectionConverter(typeof(List<>)
                    .MakeGenericType(elementType), elementType, elementConverter);
                return true;
            }

            if (typeof(LinkedList<>) == typeDef
                || typeof(Queue<>) == typeDef
                || typeof(Stack<>) == typeDef
                || typeof(List<>) == typeDef
                || typeof(HashSet<>) == typeDef
                || typeof(SortedSet<>) == typeDef
                || typeof(Dictionary<,>) == typeDef)
            {
                converters[type] = new GenericCollectionConverter(type, elementType, elementConverter);
                return true;
            }

            var enumerableDef = typeof(IEnumerable<>).MakeGenericType(elementType);
            var constructor = type.GetConstructor(new[] { enumerableDef });
            if (constructor == null) return false;
            converters[type] = new GenericCollectionConverter(type, elementType, elementConverter);
            return true;
        }

        private bool TryFallback(Type type)
        {
            var constructor = type.GetConstructor(new Type[] { typeof(string), typeof(IFormatProvider) });
            if (constructor != null)
            {
                converters[type] = new DelegatingConverter<object>(
                    (arg, format) => ParsingResult.Success(constructor.Invoke(new object[] { arg, format })));
                return true;
            }

            constructor = type.GetConstructor(new Type[] { typeof(IFormatProvider), typeof(string) });
            if (constructor != null)
            {
                converters[type] = new DelegatingConverter<object>(
                    (arg, format) => ParsingResult.Success(constructor.Invoke(new object[] { format, arg })));
                return true;
            }

            constructor = type.GetConstructor(new Type[] { typeof(string) });
            if (constructor != null)
            {
                converters[type] = new DelegatingConverter<object>(
                    (arg, _) => ParsingResult.Success(constructor.Invoke(new object[] { arg })));
                return true;
            }
            return false;
        }
    }
}