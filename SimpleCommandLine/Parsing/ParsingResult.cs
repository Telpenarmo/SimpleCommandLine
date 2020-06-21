namespace SimpleCommandLine.Parsing
{
    public abstract class ParsingResult
    {
        public abstract bool IsError { get; }
        public static ParsingResult Error(string message) => new ErrorParsingResult(message);
        public static ParsingResult Success(object result) => new SuccessfulParsingResult(result);

        public object ResultObject => (this as SuccessfulParsingResult)?.Result;
        public string ErrorMessage => (this as ErrorParsingResult)?.Message;

        private class SuccessfulParsingResult : ParsingResult
        {
            internal SuccessfulParsingResult(object result) => Result = result;
            public object Result { get; }
            public override bool IsError => false;
        }

        private class ErrorParsingResult : ParsingResult
        {
            internal ErrorParsingResult(string errorMessage) => Message = errorMessage;
            public string Message { get; }
            public override bool IsError => true;
        }
    }
}