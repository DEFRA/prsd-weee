namespace EA.Weee.XmlValidation.BusinessValidation
{
    using Core.Shared;

    public class RuleResult
    {
        public bool IsValid { get; private set; }

        public string Message { get; private set; }

        public ErrorLevel ErrorLevel { get; private set; }

        public static RuleResult Pass()
        {
            return new RuleResult();
        }

        public static RuleResult Fail(string message, ErrorLevel errorLevel = ErrorLevel.Error)
        {
            return new RuleResult(message, errorLevel);
        }

        private RuleResult()
        {
            Message = "No errors";
            IsValid = true;
        }

        private RuleResult(string message, ErrorLevel errorLevel)
        {
            IsValid = false;
            Message = message;
            ErrorLevel = errorLevel;
        }
    }
}
