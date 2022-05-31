namespace EA.Weee.Domain.Obligation
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using Error;
    using Lookup;
    using Prsd.Core.Domain;

    public partial class ObligationUploadError : Entity
    {
        public virtual ObligationUploadErrorType ErrorType { get; private set; }

        [ForeignKey("ObligationUploadId")]
        public virtual ObligationUpload ObligationUpload { get; private set; }

        public virtual Guid ObligationUploadId { get; private set; }

        public virtual string SchemeIdentifier { get; private set; }

        public virtual string SchemeName { get; private set; }

        public virtual string Description { get; private set; }

        public virtual WeeeCategory? Category { get; private set; }

        public ObligationUploadError()
        {
        }

        public ObligationUploadError(ObligationUploadErrorType errorType, string description)
        {
            ErrorType = errorType;
            Description = description;
        }

        public ObligationUploadError(ObligationUploadErrorType errorType, string schemeName, string schemeIdentifier, string description)
        {
            ErrorType = errorType;
            SchemeName = schemeName;
            SchemeIdentifier = schemeIdentifier;
            Description = description;
        }

        public ObligationUploadError(ObligationUploadErrorType errorType,
            WeeeCategory category, string schemeName, string schemeIdentifier, string description)
        {
            ErrorType = errorType;
            Category = category;
            SchemeName = schemeName;
            SchemeIdentifier = schemeIdentifier;
            Description = description;
        }
    }
}
