using System;
namespace SimpleCommandLine
{
    internal static class TypeExtensions
    {
        public static bool IsCollection(this Type type)
            => type != typeof(string) && (type.IsArray || typeof(System.Collections.IEnumerable).IsAssignableFrom(type));
    }
}