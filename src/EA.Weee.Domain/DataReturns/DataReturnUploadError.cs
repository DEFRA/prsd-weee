namespace EA.Weee.Domain.DataReturns
{
    using System;
    using Error;
    using Prsd.Core.Domain;

    public class DataReturnUploadError : Entity
    {
        public ErrorLevel ErrorLevel { get; private set; }
        public UploadErrorType ErrorType { get; private set; }
        public string Description { get; private set; }
        public int LineNumber { get; private set; }
        public Guid DataReturnUploadId { get; private set; }
        public virtual DataReturnUpload DataReturnUpload { get; private set; }
        
        public DataReturnUploadError(ErrorLevel errorLevel, UploadErrorType errorType, string description, int lineNumber = 0)
        {
            ErrorType = errorType;
            ErrorLevel = errorLevel;
            Description = description;
            LineNumber = lineNumber;
        }

        /// <summary>
        /// This constructor is used by Entity Framework.
        /// </summary>
        protected DataReturnUploadError()
        {
        }
    }
}