using System.Collections.Generic;
using SimpleCommandLine;
using static System.Console;

namespace Demo
{
    internal class FluentModeHandler : IModeHandler
    {
        public void HandleResults(Result result, List<string> list)
        {
            var opts = result.GetResult<PlainType>();
            result.WithParsed<PlainType>(x => list.Add(x.StringProp), "add")
                .WithParsed<PlainType>(x => list.RemoveAt(x.IntProp), "remove");

            WriteLine("intOption:\t" + opts.IntProp);
            WriteLine("stringOption:\t" + opts.StringProp);
        }

        public Parser RegisterArgs()
        {
            var builder = new ParserBuilder();

            builder.RegisterType<PlainType>()
                   .RegisterOption<int>((obj, i) => obj.IntProp = i, 'i')
                   .RegisterOption<string>((obj, s) => obj.StringProp = s, "sopt")
                   .RegisterValue<string[]>((obj, arr) => obj.ArrayProp = arr, 1);

            builder.RegisterType<PlainType>()
                   .AsCommand("add")
                   .RegisterValue<string>((obj, s) => obj.StringProp = s, 0);

            builder.RegisterType<PlainType>()
                   .AsCommand("remove")
                   .RegisterOption<int>((obj, i) => obj.IntProp = i, 'i', "index");

            return builder.Build();
        }
    }
}
