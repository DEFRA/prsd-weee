namespace EA.Weee.Domain.Scheme
{
    using System;
    using System.Collections.Generic;
    using Prsd.Core;
    using Prsd.Core.Domain;

    public class DataReturnsUpload : Entity
    {
        public virtual Guid SchemeId { get; private set; }
             
        public virtual Scheme Scheme { get; private set; }

        public virtual List<DataReturnsUploadError> Errors { get; private set; }

        public virtual int? ComplianceYear { get; private set; }

        public virtual bool IsSubmitted { get; private set; }

        public virtual string FileName { get; private set; }

        public virtual DateTime Date { get; private set; }
        public virtual DataReturnsUploadRawData RawData { get; set; }

        public DataReturnsUpload(string data, List<DataReturnsUploadError> errors, int? complianceYear, Guid schemeId, string fileName)
        {
            SchemeId = schemeId;
            Errors = errors;
            IsSubmitted = false;
            ComplianceYear = complianceYear;
            RawData = new DataReturnsUploadRawData() { Data = data };
            this.Date = SystemTime.UtcNow;     
        }
        public DataReturnsUpload(Guid organisationId, Guid schemeId, string data, string fileName)
        {
            SchemeId = schemeId;
            Errors = new List<DataReturnsUploadError>();
            RawData = new DataReturnsUploadRawData { Data = data };
            FileName = fileName;
            this.Date = SystemTime.UtcNow;
        }
        public void Submit()
        {
            if (IsSubmitted)
            {
                throw new InvalidOperationException("IsSubmitted status must be false to transition to true");
            }

            IsSubmitted = true;            
        }

        protected DataReturnsUpload()
        {
            this.Date = SystemTime.UtcNow;
        }
    }
}