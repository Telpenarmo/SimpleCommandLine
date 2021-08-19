using System;
using System.Collections.Generic;
using SimpleCommandLine;
using static System.Console;

namespace Demo
{
    internal class Program
    {
        private static void Main()
        {
            ShowMode(new AttributesModeHandler());
            ShowMode(new FluentModeHandler());
        }

        private static void ShowMode(IModeHandler handler)
        {
            var parser = handler.RegisterArgs();
            var result = parser.Parse(new[]
                { "-i", "42", "--sopt=add", "add", "foo", "remove", "-i", "0", "add", "bar" });

            if (result.IsError)
            {
                foreach (var e in result.Errors)
                    WriteLine(e);
                return;
            }
            var list = new List<string> { "first", "second", "third" };
            handler.HandleResults(result, list);

            Write("list:" + "\t\t");
            foreach (var item in list)
                Write(item + " ");
            WriteLine();
        }
    }


}
