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
            => converters.ContainsKey(type) || TryCreating(type) || Fallback(type);

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
                var ignoreCase = Settings.IgnoreCaseOnEnumConversion;
                var acceptNumbers = Settings.AcceptNumericalEnumValues;
                converters[type] = Attribute.IsDefined(type, typeof(FlagsAttribute))
                    ? new EnumConverter(type, ignoreCase, acceptNumbers) as IConverter
                    : new FlagsEnumConverter(type, ignoreCase, acceptNumbers);
                return true;
            }
            var fallback = System.ComponentModel.TypeDescriptor.GetConverter(type);
            var success = fallback.CanConvertFrom(typeof(string));
            if (success)
                converters[type] = new TypeDescriptorConverter(fallback);

            return success;
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
            Type elementType = type.GetCollectionElementType();
            if (!CheckForType(elementType))
                return false; // failure when element's type is not convertable

            return TryArray(type, elementType)
                || (type.IsGenericType && TryGenericCollection(type, elementType))
                || TryConstructingCollection(type, elementType);
        }

        private bool TryArray(Type type, Type elementType)
        {
            if (!(type.IsArray || type.IsAssignableFrom(typeof(Array))))
                return false;
            converters[type] = new ArrayConverter(elementType, this[elementType]);
            return true;
        }

        private bool TryGenericCollection(Type type, Type elementType)
        {
            var typeDef = type.GetGenericTypeDefinition();

            if (typeof(LinkedList<>) == typeDef
                || typeof(Queue<>) == typeDef
                || typeof(Stack<>) == typeDef
                || typeof(List<>) == typeDef
                || typeof(HashSet<>) == typeDef
                || typeof(SortedSet<>) == typeDef
                || typeof(Dictionary<,>) == typeDef)
            {
                converters[type] = new GenericCollectionConverter(type, elementType, this[elementType]);
                return true;
            }

            if (type.IsInterface &&
               (typeof(IList<>) == typeDef
               || typeof(IEnumerable<>) == typeDef
               || typeof(ICollection<>) == typeDef
               || typeof(IReadOnlyCollection<>) == typeDef
               || typeof(IReadOnlyList<>) == typeDef))
            {
                converters[type] = new GenericCollectionConverter(typeof(List<>)
                    .MakeGenericType(elementType), elementType, this[elementType]);
                return true;
            }

            return false;
        }

        private bool TryConstructingCollection(Type type, Type elementType)
        {
            var enumerableDef = typeof(IEnumerable<>).MakeGenericType(elementType);
            var constructor = type.GetConstructor(new[] { enumerableDef });
            if (constructor == null) return false;
            converters[type] = new GenericCollectionConverter(type, elementType, this[elementType]);
            return true;
        }

        private bool Fallback(Type type)
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