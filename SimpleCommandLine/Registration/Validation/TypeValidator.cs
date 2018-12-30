using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleCommandLine.Registration.Validation
{
    internal class TypeValidator : ITypeValidator
    {
        private readonly IEnumerable<ParsingTypeInfo> types;

        public TypeValidator(IEnumerable<ParsingTypeInfo> types)
        {
            this.types = types ?? throw new ArgumentNullException(nameof(types));
        }

        public bool Verify(ParsingTypeInfo typeInfo, IList<OptionAttribute> optionAttributes)
        {
            if (typeInfo.Values.Select(x => x.Index).HasDuplicates())
                throw new InvalidOperationException("Values must have different indices.");
            if (optionAttributes.HasDuplicates())
                throw new InvalidOperationException("Options must have different short names.");
            if (optionAttributes.HasDuplicates())
                throw new InvalidOperationException("Options must have different long names.");
            if (!(typeInfo is ParsingCommandTypeInfo) && !typeInfo.Options.Any() && !typeInfo.Values.Any())
                throw new InvalidOperationException("Non-command type must have options or values.");
            if (typeInfo is ParsingCommandTypeInfo command && types.OfType<ParsingCommandTypeInfo>().SelectMany(x => x.Aliases).HasDuplicates(command.Aliases))
                throw new InvalidOperationException("One of this command's name has been already registered");
            else
                return true;
        }
    }

    public static class EnumerableExtensions
    {
        public static bool HasDuplicates<T>(this IEnumerable<T> subjects)
        {
            if (subjects == null)
                throw new ArgumentNullException(nameof(subjects));

            var set = new HashSet<T>();

            foreach (var s in subjects)
                if (!set.Add(s))
                    return true;

            return false;
        }

        public static bool HasDuplicates<T>(this IEnumerable<T> subjects, IEnumerable<T> second)
        {
            if (subjects == null)
                throw new ArgumentNullException(nameof(subjects));
            if (second == null)
                throw new ArgumentNullException(nameof(second));

            var set = new HashSet<T>(subjects);

            foreach (var s in second)
                if (!set.Add(s))
                    return true;

            return false;
        }
    }
}