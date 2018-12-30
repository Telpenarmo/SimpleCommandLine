# SimpleCommandLine

SimpleCommandLine is a strongly-typed library for parsing console arguments, that works best in multi-usage scenario, since most work is done in separate Builder class.

I have made SimpleCommandLine mostly for learning purposes, therefore for now it is not ready for production use. However I am going to gradually enhance it.

## Getting Started

First you have to create a *ParserBuilder* and register at most one generic type and your command types. You can also configure the parser in that moment.

### Usage

#### Simple (No Commands) Scenario

```cs
using System;
using SimpleCommandLine;

class Program
{
    static void Main(string[] args)
    {
        var builder = new ParserBuilder();
        builder.RegisterType<Options>();
        var parser = builder.Build();
        var result = parser.Parse(new[] { "-o", "someValue" });
        result.WithParsed<Options>(x => Console.WriteLine(x.SomeStringOption));
        //prints: someValue
    }
}

class Options
{
    [Option(ShortName = 'o')]
    public string SomeStringOption { get; set; }
}
```

#### Commands Scenario

```cs
using System;
using System.Collections.Generic;
using SimpleCommandLine;

class Program
{
    static void Main(string[] args)
    {
        List<string> list = new List<string>(){"one", "two", "three"};

        var builder = new ParserBuilder();
        builder.RegisterType<RemoveCommand>();
        builder.RegisterType<AddCommand>();

        var parser = builder.Build();
        parser.Parse(new[]{ "remove", "--index", "0" });
        var result = parser.Parse(new[] { "add", "four" });
        result.WithParsed<AddCommand>(x => list.Add(x.Item))
            .WithParsed<RemoveCommand>(x => list.RemoveAt(x.Index));

        foreach (var item in list)
            Console.WriteLine(item);
        /*prints:
        two
        three
        four
        */
    }
}

[Command("remove")]
class RemoveCommand
{
    [Option(LongName = "index")]
    public int Index { get; set; }
}

[Command("add")]
class AddCommand
{
    [Value(0)]
    public string Item { get; set; }
}
```

## Newbie Alert!

Due to lack of my experience this project is barely tested and probably full of bugs, it is also deficient in features. However if you find my project interesting, feel free to use it and report every issue you'll experience. I will be also very grateful for suggestions about features as well as their implementation and any kind of contribution.