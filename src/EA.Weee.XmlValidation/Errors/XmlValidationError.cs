namespace EA.Weee.XmlValidation.Errors
{
    using Core.Shared;

    public class XmlValidationError
    {
        public ErrorLevel ErrorLevel { get; private set; }

        public XmlErrorType ErrorType { get; private set; }

        public string Message { get; private set; }

        public XmlValidationError(ErrorLevel errorLevel, XmlErrorType errorType, string message)
        {
            ErrorLevel = errorLevel;
            ErrorType = errorType;
            Message = message;
        }
    }
}
