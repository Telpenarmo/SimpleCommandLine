using System.Collections.Generic;

namespace SimpleCommandLine.Registration.Validation
{
    internal interface ITypeValidator
    {
        bool Verify(ParsingTypeInfo typeInfo, IList<OptionAttribute> optionAttributes);
    }
}