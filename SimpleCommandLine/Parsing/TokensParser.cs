using System;
using System.Collections.Generic;
using System.Linq;
using SimpleCommandLine.Tokens;

namespace SimpleCommandLine.Parsing
{
    /// <summary>
    /// Manages the single parsing proccess.
    /// </summary>
    public class TokensParser
    {
        private readonly IResultBuilderFactory objectBuilderFactory;
        private readonly Dictionary<string, object> results = new Dictionary<string, object>();
        private readonly List<string> errors = new List<string>();
        private ResultBuilder builder;
        private string lastCommandUsed = "";
        private bool ErrorOccured => errors.Count != 0;

        internal TokensParser(IResultBuilderFactory objectBuilderFactory)
        {
            this.objectBuilderFactory = objectBuilderFactory;
        }

        /// <summary>
        /// Parses a collection of arguments.
        /// </summary>
        /// <param name="args">Collection of command-line arguments to be parsed.</param>
        /// <returns>Instance of the <see cref="Result"/> class that is used to consume the parsing result.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public Result Parse(IEnumerable<IArgumentToken> tokens)
        {
            builder = objectBuilderFactory.Build();

            if (builder is null && !(tokens.FirstOrDefault() is CommandToken))
                return Result.Error(new[] { "Generic type was not provided!" });

            foreach (var token in tokens)
            {
                switch (token)
                {
                    case CommandToken command:
                        if (builder != null) NewResult();
                        builder = objectBuilderFactory.Build(command.Name);
                        lastCommandUsed = command.Name; 
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

            return ErrorOccured ? Error() : Result.Success(results);
        }

        private void NewResult()
        {
            ParsingResult result = builder.Parse();
            if (result.IsError)
                errors.Add(result.ErrorMessage);
            else
                results[lastCommandUsed] = result.ResultObject;
        }

        private Result Error() => Result.Error(errors);

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