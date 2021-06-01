using System.Collections.Generic;

namespace SimpleCommandLine.Parsing
{
    public abstract class ParsingResult
    {
        public abstract bool IsError { get; }
        public static ParsingResult Error(string message) => new ErrorParsingResult(new[] { message });
        public static ParsingResult Error(IEnumerable<string> messages) => new ErrorParsingResult(messages);
        public static ParsingResult Success(object? result) => new SuccessfulParsingResult(result);

        public object? ResultObject => (this as SuccessfulParsingResult)?.Result;
        public IEnumerable<string>? ErrorMessages => (this as ErrorParsingResult)?.Messages;

        private class SuccessfulParsingResult : ParsingResult
        {
            internal SuccessfulParsingResult(object? result) => Result = result;
            public object? Result { get; }
            public override bool IsError => false;
        }

        private class ErrorParsingResult : ParsingResult
        {
            internal ErrorParsingResult(IEnumerable<string> errorMessages) => Messages = errorMessages;
            public IEnumerable<string> Messages { get; }
            public override bool IsError => true;
        }
    }
}