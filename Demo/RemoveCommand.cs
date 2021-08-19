using System;
using SimpleCommandLine;

namespace Demo
{
    [Command("remove")]
    internal class RemoveCommand
    {
        [Option(LongName = "index", ShortName = 'i')]
        public int Index { get; set; }
    }
}