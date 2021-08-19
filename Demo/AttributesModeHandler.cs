using System.Collections.Generic;
using SimpleCommandLine;
using static System.Console;

namespace Demo
{
    public class AttributesModeHandler : IModeHandler
    {
        public void HandleResults(Result result, List<string> list)
        {
            var opts = result.GetResult<GlobalOptions>();
            result.WithParsed<AddCommand<string>>(x => list.Add(x.Item))
                .WithParsed<RemoveCommand>(x => list.RemoveAt(x.Index));

            WriteLine("intOption:\t" + opts.IntOption);
            WriteLine("stringOption:\t" + opts.StringOption);
        }

        public Parser RegisterArgs()
        {
            var builder = new ParserBuilder();
            builder.RegisterType<GlobalOptions>();
            builder.RegisterType<AddCommand<string>>();
            builder.RegisterType<RemoveCommand>();
            return builder.Build();
        }
    }
}