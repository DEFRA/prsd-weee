namespace EA.Weee.Domain
{
    using System;
    using EA.Prsd.Core.Domain;

    public class MemberUploadError : Entity
    {
        public ErrorLevel ErrorLevel { get; private set; }

        public string Description { get; private set; }

        public Guid MemberUploadId { get; set; }
        public virtual MemberUpload MemberUpload { get; set; }

        public MemberUploadError(ErrorLevel errorLevel, string description)
        {
            ErrorLevel = errorLevel;
            Description = description;
        }

        protected MemberUploadError()
        {
        }
    }
}