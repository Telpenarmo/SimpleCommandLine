using SimpleCommandLine.Parsing;
using System;
using System.Collections.Generic;

namespace SimpleCommandLine.Registration.Validation
{
    internal interface IFinalValidator
    {
        void Verify(IEnumerable<ParsingTypeInfo> types, IValueConvertersFactory convertersFactory);
    }
}