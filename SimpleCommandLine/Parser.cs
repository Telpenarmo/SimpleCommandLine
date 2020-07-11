using System;
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
        private List<object> results = new List<object>();
        private List<string> errors = new List<string>();
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
            var tokens = (args ?? Enumerable.Empty<string>())
                .Select(arg => tokenizer.TokenizeArgument(arg));

            if (builder is null && !(tokens.FirstOrDefault() is CommandToken))
                return Result.Error(new[] { "Generic type was not provided!" });

            foreach (var token in tokens)
            {
                switch (token)
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
                    case AssignedValueToken assignedValue:
                        HandleOption(assignedValue.Option);
                        builder.LastAssignedOption.SetValue(assignedValue.Value);
                        break;

                }
                if (ErrorOccured) return Error();
            }
            EnsureLastOptionSet();
            NewResult();

            return ErrorOccured ? Error() : Success();
        }

        private void NewResult()
        {
            if (builder is null || ErrorOccured) return;
            ParsingResult result = builder.Parse();
            if (result.IsError)
                errors.Add(result.ErrorMessage);
            else
                results.Add(result.ResultObject);
        }

        private Result Error()
        {
            var old = errors;
            Reset();
            return Result.Error(old);
        }

        private Result Success()
        {
            var old = results;
            Reset();
            return Result.Success(old);
        }

        private void Reset()
        {
            errors = new List<string>();
            results = new List<object>();
            builder = null;
        }

        protected void HandleOption(OptionToken token)
        {
            EnsureLastOptionSet();
            if (!builder.TryAddOption(token))
                errors.Add($"The current type does not contain the \"{token}\" option.");
        }

        protected void HandleValue(ValueToken token)
        {
            if (builder.LastAssignedOption?.AcceptsValue ?? false)
                builder.LastAssignedOption.AddValue(token);
            else if (builder.AwaitsValue)
                builder.AddValue(token);
            else
                errors.Add("The current type does not accept any more values.");
        }

        private void EnsureLastOptionSet()
        {
            if (builder.LastAssignedOption?.RequiresValue ?? false)
                errors.Add("Value was not provided for an option.");
        }
    }
}