namespace EA.Weee.Domain.Obligation
{
    using Error;
    using Lookup;

    public class ObligationUploadError
    {
        public ObligationUploadErrorType Error { get; private set; }

        public string SchemeIdentifier { get; private set; }

        public string SchemeName { get; private set; }

        public string Description { get; private set; }

        public WeeeCategory Category { get; private set; }

        public ObligationUploadError(ObligationUploadErrorType error, string description)
        {
            Error = error;
            Description = description;
        }

        public ObligationUploadError(ObligationUploadErrorType error, string schemeName, string schemeIdentifier, string description)
        {
            Error = error;
            SchemeName = schemeName;
            SchemeIdentifier = schemeIdentifier;
            Description = description;
        }

        public ObligationUploadError(ObligationUploadErrorType error,
            WeeeCategory category, string schemeName, string schemeIdentifier, string description)
        {
            Error = error;
            Category = category;
            SchemeName = schemeName;
            SchemeIdentifier = schemeIdentifier;
            Description = description;
        }
    }
}
