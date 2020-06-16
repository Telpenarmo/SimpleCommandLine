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
            var enumerator = (args ?? Enumerable.Empty<string>())
                .Select(arg => tokenizer.TokenizeArgument(arg)).GetEnumerator();

            if (builder is null && !(enumerator.Current is CommandToken))
                throw new ArgumentException("Generic type was not provided!");

            while (enumerator.MoveNext())
            {
                switch (enumerator.Current)
                {
                    case CommandToken command:
                        NewResult();
                        builder = objectBuilderFactory.Build(command.Name);
                        break;
                    case OptionsGroupToken group:
                        group.Tokens.ForEach(o => HandleOption(o));
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
            NewResult();

            return new Result(results);
        }

        private void NewResult() => results.Add(builder.Parse());
        protected void HandleOption(OptionToken token)
        {
            EnsureLastOptionSet();
            builder.AddOption(token);
        }

        protected void HandleValue(ValueToken token)
        {
            if (builder.LastAssignedOption?.AcceptsValue ?? false)
                builder.LastAssignedOption.AddValue(token);
            else if (builder.AwaitsValue)
                builder.AddValue(token);
            else
                throw new ArgumentException("This type does not accept any more values.");
        }

        private void EnsureLastOptionSet()
        {
            if (builder.LastAssignedOption?.RequiresValue ?? false)
                throw new ArgumentException("Value was not provided for a token.");
        }
    }
}