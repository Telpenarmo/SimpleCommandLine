using System;
using System.Collections.Generic;
using SimpleCommandLine;

namespace Demo
{
    internal interface IModeHandler
    {
        Parser RegisterArgs();
        void HandleResults(Result result, List<string> list);
    }
}
