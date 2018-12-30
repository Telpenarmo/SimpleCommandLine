using System;
using System.Collections.Generic;
using System.Linq;
using SimpleCommandLine.Parsing;

namespace SimpleCommandLine.Registration.Validation
{
    internal class FinalValidator : IFinalValidator
    {
        public void Verify(IEnumerable<ParsingTypeInfo> types, IValueConvertersFactory convertersFactory)
        {
            if (types.Where(x => !(x is ParsingCommandTypeInfo)).Count() > 1)
                throw new InvalidOperationException("You can register only one non-command type");

            foreach (var type in types.SelectMany(x => x.Options.OfType<ParsingArgumentInfo>().Concat(x.Values).Select(o => o.PropertyType)))
            {
                if (! convertersFactory.CanConvert(type))
                    throw new InvalidOperationException($"No converter is specified for the {type.FullName} type.");
            }
        }
    }
}