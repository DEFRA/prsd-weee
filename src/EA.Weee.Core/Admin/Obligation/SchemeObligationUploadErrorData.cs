namespace EA.Weee.Core.Admin.Obligation
{
    using Domain.Lookup;

    public class SchemeObligationUploadErrorData
    {
        public SchemeObligationUploadErrorType ErrorType { get; private set; }

        public string Description { get; private set; }

        public string SchemeIdentifier { get; private set; }

        public string Scheme { get; private set; }

        public WeeeCategory? Category { get; private set; }

        public SchemeObligationUploadErrorData(SchemeObligationUploadErrorType errorType,
            string description, string schemeIdentifier, string scheme, WeeeCategory? category)
        {
            ErrorType = errorType;
            Description = description;
            SchemeIdentifier = schemeIdentifier;
            Scheme = scheme;
            Category = category;
        }
    }
}
