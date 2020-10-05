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
        private ResultBuilder? builder;
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

            foreach (var token in tokens)
            {
                if (token is CommandToken command)
                {
                    if (builder != null) NewResult();
                    builder = objectBuilderFactory.Build(command.Name);
                    lastCommandUsed = command.Name;
                }
                else builder?.HandleToken(token);
                if (ErrorOccured) return Error();
            }
            NewResult();

            return ErrorOccured ? Error() : Result.Success(results);
        }

        private void NewResult()
        {
            ParsingResult? result = builder?.Build();
            if (result is null)
                errors.Add("Generic type was not provided!");
            else if (result.ErrorMessages != null)
                errors.AddRange(result.ErrorMessages);
            else if (result.ResultObject != null)
                results[lastCommandUsed] = result.ResultObject;
        }

        private Result Error() => Result.Error(errors);
    }
}