using System;
using System.Collections.Generic;
using SimpleCommandLine;
using static System.Console;

namespace Demo
{
    internal class Program
    {
        private static List<string> list;
        private static void Main()
        {
            list = new() { "first", "second", "third" };
            UseAttributesMode();
            list = new() { "first", "second", "third" };
            UseFluentRegistratingMode();
        }

        private static void UseAttributesMode()
        {
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
            {
                foreach (var e in result.Errors)
                    WriteLine(e);
                return;
            }
            WriteLine("intOption:\t" + opts.IntOption);
            WriteLine("stringOption:\t" + opts.StringOption);
            Write("list:" + "\t\t");
            foreach (var item in list)
                Write(item + " ");
            WriteLine();
        }

        private static void UseFluentRegistratingMode()
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

            var parser = builder.Build();

            var result = parser.Parse(new[]
                { "-i", "42", "--sopt=add", "add", "foo", "remove", "-i", "0", "add", "bar" });

            var opts = result.GetResult<PlainType>();
            result.WithParsed<PlainType>(x => list.Add(x.StringProp), "add")
                .WithParsed<PlainType>(x => list.RemoveAt(x.IntProp), "remove");

            if (result.IsError)
            {
                foreach (var e in result.Errors)
                    WriteLine(e);
                return;
            }
            WriteLine("intOption:\t" + opts.IntProp);
            WriteLine("stringOption:\t" + opts.StringProp);
            Write("list:" + "\t\t");
            foreach (var item in list)
                Write(item + " ");
            WriteLine();
        }
    }


}
