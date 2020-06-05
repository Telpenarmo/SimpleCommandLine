using System;
using System.Collections.Generic;
using System.Linq;
using SimpleCommandLine.Parsing;
using SimpleCommandLine.Tokenization.Tokens;
using SimpleCommandLine.Tokenization.Tokenizers;

namespace SimpleCommandLine
{
    /// <summary>
    /// Used to parse command-line arguments.
    /// </summary>
    public class Parser
    {
        private readonly ObjectBuilderFactory objectBuilderFactory;
        private ObjectBuilder builder;
        private readonly IArgumentTokenizer tokenizer;
        private readonly List<object> results = new List<object>();

        internal Parser(IArgumentTokenizer tokenizer, ObjectBuilderFactory objectBuilderFactory)
        {
            this.tokenizer = tokenizer;
            this.objectBuilderFactory = objectBuilderFactory;
        }

        /// <summary>
        /// Parses a collection of arguments.
        /// </summary>
        /// <param name="args">Collection of command-line arguments to be parsed.</param>
        /// <returns>Instance of the <see cref="Result"/> class that is used to consume the parsing result.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public Result Parse(IEnumerable<string> args)
        {
            builder = objectBuilderFactory.Build();
            var tokens = new Queue<IArgumentToken>((args ?? Enumerable.Empty<string>())
                .Select(a => tokenizer.TokenizeArgument(a)));
            if (builder is null && !(tokens.Peek() is CommandToken))
                throw new ArgumentException("Generic type was not provided!");
            while (tokens.Any())
            {
                var current = tokens.Dequeue();
                switch (current)
                {
                    case CommandToken command:
                        results.Add(builder.Parse());
                        builder = objectBuilderFactory.Build(command.Name);
                        break;
                    case OptionToken option:
                        HandleOption(option);
                        break;
                    case ValueToken value:
                        HandleValue(value);
                        break;
                }
            }
            EnsureLastOptionSet();

            return new Result(results);
        }

        protected void HandleOption(OptionToken token)
        {
            EnsureLastOptionSet();
            builder.AddOption(token);
        }

        protected void HandleValue(ValueToken token)
        {
            if (builder.LastOption?.AcceptsValue ?? false)
                builder.LastOption.AddValue(token);
            else if (builder.AwaitsValue)
                builder.AddValue(token);
            else
                throw new ArgumentException("This type does not accept any more values.");
        }

        private void EnsureLastOptionSet()
        {
            if (builder.LastOption?.RequiresValue ?? false)
                throw new ArgumentException("Value was not provided for a token.");
        }
    }
}