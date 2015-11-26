namespace EA.Weee.XmlValidation.Errors
{
    using Core.Shared;

    public class XmlValidationError
    {
        public ErrorLevel ErrorLevel { get; private set; }

        public XmlErrorType ErrorType { get; private set; }

        public string Message { get; private set; }

        public int? LineNumber { get; private set; }

        public XmlValidationError(ErrorLevel errorLevel, XmlErrorType errorType, string message, int? lineNumber = null)
        {
            ErrorLevel = errorLevel;
            ErrorType = errorType;
            Message = message;
            LineNumber = lineNumber;
        }
    }
}
