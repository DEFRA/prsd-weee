namespace EA.Weee.Domain.PCS
{
    using System;
    using Prsd.Core.Domain;

    public class MemberUploadError : Entity
    {
        public ErrorLevel ErrorLevel { get; private set; }
        public MemberUploadErrorType ErrorType { get; private set; }

        public string Description { get; private set; }

        public Guid MemberUploadId { get; private set; }
        public virtual MemberUpload MemberUpload { get; private set; }

        public MemberUploadError(ErrorLevel errorLevel, MemberUploadErrorType errorType, string description)
        {
            ErrorType = errorType;
            ErrorLevel = errorLevel;
            Description = description;
        }

        protected MemberUploadError()
        {
        }
    }
}