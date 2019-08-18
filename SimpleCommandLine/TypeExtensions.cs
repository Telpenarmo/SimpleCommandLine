using System;

namespace SimpleCommandLine
{
    internal static class TypeExtensions
    {
        public static bool IsCollection(this Type type)
            => type != typeof(string) && (type.IsArray || typeof(System.Collections.IEnumerable).IsAssignableFrom(type));
        public static Type GetCollectionElementType(this Type type)
        {
            if (type.IsArray)
                return type.GetElementType();
            else if (typeof(System.Collections.IEnumerable).IsAssignableFrom(type))
                return type.GenericTypeArguments[0];
            else return null;
        }
    }
}