namespace EA.Weee.Domain.Scheme
{
    using System;
    using Prsd.Core.Domain;

    public class MemberUploadError : Entity
    {
        public ErrorLevel ErrorLevel { get; private set; }
        public MemberUploadErrorType ErrorType { get; private set; }
        public string Description { get; private set; }
        public int LineNumber { get; private set; }
        public Guid MemberUploadId { get; private set; }
        public virtual MemberUpload MemberUpload { get; private set; }
        
        public MemberUploadError(ErrorLevel errorLevel, MemberUploadErrorType errorType, string description, int lineNumber = 0)
        {
            ErrorType = errorType;
            ErrorLevel = errorLevel;
            Description = description;
            LineNumber = lineNumber;
        }

        protected MemberUploadError()
        {
        }
    }
}