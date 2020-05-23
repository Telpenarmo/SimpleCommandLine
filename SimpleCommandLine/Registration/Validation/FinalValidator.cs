using System;
using System.Collections.Generic;
using System.Linq;
using SimpleCommandLine.Parsing;

namespace SimpleCommandLine.Registration.Validation
{
    internal class FinalValidator
    {
        public void Verify(IEnumerable<ParsingTypeInfo> types, IConvertersFactory convertersFactory)
        {
            if (types.Where(x => !(x is ParsingCommandTypeInfo)).Count() > 1)
                throw new InvalidOperationException("You can register only one non-command type");

            if (types.SelectMany(x => x.Options.OfType<ParsingArgumentInfo>().Concat(x.Values).Select(p => p.ChooseConverter(convertersFactory) == null)).Any())
            {
                throw new InvalidOperationException($"No converter is specified for a type.");
            }
        }
    }
}