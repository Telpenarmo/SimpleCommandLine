﻿using System;
using System.Collections.Generic;
using System.Linq;
using SimpleCommandLine.Parsing;
using SimpleCommandLine.Tokens;
using SimpleCommandLine.Tokenizers;

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
        private readonly List<string> errors = new List<string>();
        private bool ErrorOccured => errors.Count != 0;

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
                return new Result(new[] { "Generic type was not provided!" });

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
                if (ErrorOccured) return new Result(errors);
            }
            EnsureLastOptionSet();
            NewResult();

            return ErrorOccured ? new Result(errors) : new Result(results);
        }
        
        private void NewResult()
        {
            if (ErrorOccured) return;
            ParsingResult result = builder.Parse();
            if (result.IsError)
                errors.Add(result.AsError.ErrorMessage);
            results.Add(result.AsSuccess.Result);
        }

        protected void HandleOption(OptionToken token)
        {
            EnsureLastOptionSet();
            if (!builder.TryAddOption(token))
                errors.Add($"This type does not contain the {token} option.");
        }

        protected void HandleValue(ValueToken token)
        {
            if (builder.LastAssignedOption?.AcceptsValue ?? false)
                builder.LastAssignedOption.AddValue(token);
            else if (builder.AwaitsValue)
                builder.AddValue(token);
            else
                errors.Add("This type does not accept any more values.");
        }

        private void EnsureLastOptionSet()
        {
            if (builder.LastAssignedOption?.RequiresValue ?? false)
                errors.Add("Value was not provided for an option.");
        }
    }
}