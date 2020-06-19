namespace SimpleCommandLine.Parsing
{
    public abstract class ParsingResult
    {
        public abstract bool IsError { get; }
        public static ErrorParsingResult Error(string message) => new ErrorParsingResult(message);
        public static SuccessfulParsingResult Success(object result) => new SuccessfulParsingResult(result);

        public SuccessfulParsingResult AsSuccess => this as SuccessfulParsingResult;
        public ErrorParsingResult AsError => this as ErrorParsingResult;
        
        public class SuccessfulParsingResult : ParsingResult
        {
            internal SuccessfulParsingResult(object result) => Result = result;
            public object Result { get; }
            public override bool IsError => false;
        }

        public class ErrorParsingResult : ParsingResult
        {
            internal ErrorParsingResult(string errorMessage) => ErrorMessage = errorMessage;
            public string ErrorMessage { get; }
            public override bool IsError => true;
        }
    }
}