using System.Collections.Generic;
using SimpleCommandLine.Registration;

namespace SimpleCommandLine.Tests.Fakes
{
    internal class TestObject
    {
        [Value(1)]
        public string StringValue { get; set; }

        [Value(2)]
        public int IntValue { get; set; }

        [Value(3)]
        public Dictionary<int, string> DictionaryValue { get; set; }

        [Option(LongName = "sOption", ShortName = 's')]
        public string StringOption { get; set; }

        [Option(LongName = "iOption", ShortName = 'i')]
        public int IntOption { get; set; }

        [Option(LongName = "1Option", ShortName = '1')]
        public bool BoolOption1 { get; set; }

        [Option(LongName = "2Option", ShortName = '2')]
        public bool BoolOption2 { get; set; }

        [Option(LongName = "3Option", ShortName = '3')]
        public bool BoolOption3 { get; set; }
        
        [Option(LongName = "lOption", ShortName = 'l')]
        public List<string> ListOption { get; set; }

        [Option(LongName = "aOption", ShortName = 'a')]
        public string[] ArrayOption { get; set; }

        public static IEnumerable<ParsingValueInfo> ValueInfos => new[]
            {
                new ParsingValueInfo(typeof(TestObject).GetProperty(nameof(StringValue))),
                new ParsingValueInfo(typeof(TestObject).GetProperty(nameof(IntValue))),
                //new ParsingValueInfo(typeof(TestObject).GetProperty(nameof(DictionaryValue))),
            };

        public static IEnumerable<ParsingOptionInfo> OptionsInfos => new[]
            {
                new ParsingOptionInfo(typeof(TestObject).GetProperty(nameof(StringOption))),
                new ParsingOptionInfo(typeof(TestObject).GetProperty(nameof(IntOption))),
                new ParsingOptionInfo(typeof(TestObject).GetProperty(nameof(BoolOption1))),
                new ParsingOptionInfo(typeof(TestObject).GetProperty(nameof(BoolOption2))),
                //new ParsingOptionInfo(typeof(TestObject).GetProperty(nameof(ListOption))),
                //new ParsingOptionInfo(typeof(TestObject).GetProperty(nameof(ArrayOption))),
            };
    }
}
