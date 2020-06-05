using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleCommandLine.Registration.Validation
{
    internal class TypeValidator
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
            if (optionAttributes.Select(attr => attr.ShortName).HasDuplicates())
                throw new InvalidOperationException("Options must have different short names.");
            if (optionAttributes.Select(attr => attr.LongName).HasDuplicates())
                throw new InvalidOperationException("Options must have different long names.");
            if (typeInfo.Name != "" && types.Select(x => x.Name).Contains(typeInfo.Name))
                throw new InvalidOperationException("One of this command's name has been already registered");
            else
                return true;
        }
    }
}