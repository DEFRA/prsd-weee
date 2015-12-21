namespace EA.Weee.Core.Shared
{
    public class ErrorData
    {
        public ErrorData(string description, ErrorLevel errorLevel)
        {
            ErrorLevel = errorLevel;
            Description = description;
        }

        public ErrorLevel ErrorLevel { get; private set; }

        public string Description { get; private set; }
    }
}