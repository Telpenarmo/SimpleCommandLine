using System;
using System.Collections.Generic;
using static System.Console;
using SimpleCommandLine;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var list = new List<string>() { "first", "second", "third" };
            var builder = new ParserBuilder();
            builder.RegisterType<GlobalOptions>();
            builder.RegisterType<AddCommand<string>>();
            builder.RegisterType<RemoveCommand>();
            var parser = builder.Build();

            var result = parser.Parse(new[]
                { "-i", "42", "--sopt=add", "add", "foo", "remove", "-i", "0", "add", "bar" });

            var opts = result.GetResult<GlobalOptions>();
            result.WithParsed<AddCommand<string>>(x => list.Add(x.Item))
                .WithParsed<RemoveCommand>(x => list.RemoveAt(x.Index));

            if (result.IsError)
                foreach (var e in result.Errors)
                    WriteLine(e);
            WriteLine("intOption:\t" + opts.IntOption);
            WriteLine("stringOption:\t" + opts.StringOption);
            Write("list:" + "\t\t");
            foreach (var x in list)
                Write(x + " ");
            WriteLine();
        }
    }
}
