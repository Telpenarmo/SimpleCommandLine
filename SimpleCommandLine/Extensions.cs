﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SimpleCommandLine
{
    internal static class TypeExtensions
    {
        public static bool IsCollection(this Type type)
            => type != typeof(string) && (type.IsArray || typeof(IEnumerable).IsAssignableFrom(type));

        public static Type GetCollectionElementType(this Type type)
        {
            if (type.IsArray)
                return type.GetElementType();
            else if (typeof(IEnumerable).IsAssignableFrom(type))
                return type.IsGenericType ? type.GenericTypeArguments[0] : typeof(object);
            else throw new InvalidOperationException("Non-collection type.");
        }

        public static bool IsTuple(this Type type)
        {
            if (type == typeof(DictionaryEntry)) return true;
            if (!type.IsGenericType) return false;
            var def = type.GetGenericTypeDefinition();
            return def == typeof(Tuple) || def == typeof(KeyValuePair<,>) || def == typeof(ValueTuple);
        }

        public static Type[] GetTupleElementTypes(this Type type)
        {
            if (type == typeof(DictionaryEntry)) return new[] { typeof(object), typeof(object) };
            if (!type.IsGenericType) throw new InvalidOperationException("Non-tuple type.");
            return type.GetGenericArguments();
        }
    }

    internal static class EnumerableExtensions
    {
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var element in collection)
                action(element);
            return collection;
        }
    }
}