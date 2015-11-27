namespace EA.Weee.Domain.Scheme
{
    using Prsd.Core.Domain;
    using System;

    public class DataReturnsUploadError : Entity
    {
        public ErrorLevel ErrorLevel { get; private set; }
        public UploadErrorType ErrorType { get; private set; }
        public string Description { get; private set; }
        public int LineNumber { get; private set; }
        public Guid DataReturnsUploadId { get; private set; }
        public virtual DataReturnsUpload DataReturnsUpload { get; private set; }
        
        public DataReturnsUploadError(ErrorLevel errorLevel, UploadErrorType errorType, string description, int lineNumber = 0)
        {
            ErrorType = errorType;
            ErrorLevel = errorLevel;
            Description = description;
            LineNumber = lineNumber;
        }

        protected DataReturnsUploadError()
        {
        }
    }
}