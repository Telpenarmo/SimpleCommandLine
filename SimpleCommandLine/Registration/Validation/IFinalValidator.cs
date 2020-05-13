using System.Collections.Generic;
using SimpleCommandLine.Parsing;

namespace SimpleCommandLine.Registration.Validation
{
    internal interface IFinalValidator
    {
        void Verify(IEnumerable<ParsingTypeInfo> types, IConvertersFactory convertersFactory);
    }
}