namespace EA.Weee.Domain.DataReturns
{
    using System;
    using System.Collections.Generic;
    using Prsd.Core;
    using Prsd.Core.Domain;
    using Scheme;

    public class DataReturnsUpload : Entity
    {
        public virtual Scheme Scheme { get; private set; }

        public virtual List<DataReturnsUploadError> Errors { get; private set; }

        public virtual int? ComplianceYear { get; private set; }

        public virtual bool IsSubmitted { get; private set; }

        public virtual string FileName { get; private set; }

        public virtual DateTime Date { get; private set; }

        public virtual DataReturnsUploadRawData RawData { get; set; }

        public virtual TimeSpan ProcessTime { get; private set; }

        public DataReturnsUpload(string data, List<DataReturnsUploadError> errors, int? complianceYear, Scheme scheme, string fileName)
        {
            Scheme = scheme;
            Errors = errors;
            RawData = new DataReturnsUploadRawData() { Data = data };
            this.Date = SystemTime.UtcNow;
            FileName = fileName;
            IsSubmitted = false;
            ComplianceYear = complianceYear;            
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

        public virtual void SetProcessTime(TimeSpan processTime)
        {
            if (ProcessTime.Ticks.Equals(0))
            {
                ProcessTime = processTime;
            }
            else
            {
                throw new InvalidOperationException("ProcessTime cannot be set for a Data returns upload that has already been given a ProcessTime value.");
            }
        }
    }
}