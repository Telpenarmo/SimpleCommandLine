using System;
using SimpleCommandLine;

namespace Demo
{
    internal class GlobalOptions
    {
        [Option(ShortName = 'i')]
        public int IntOption { get; set; }

        [Option(LongName = "sopt")]
        public string StringOption { get; set; }

        [Value(1)]
        public string[] Args { get; set; }
    }
}