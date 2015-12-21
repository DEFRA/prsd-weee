namespace EA.Weee.Domain.DataReturns
{
    using System;
    using System.Collections.Generic;
    using Prsd.Core;
    using Prsd.Core.Domain;
    using Scheme;

    public class DataReturnUpload : Entity
    {
        public virtual Scheme Scheme { get; private set; }

        public virtual int? ComplianceYear { get; private set; }

        public virtual int? Quarter { get; private set; }

        public virtual List<DataReturnUploadError> Errors { get; private set; }

        public virtual string FileName { get; private set; }

        public virtual DateTime Date { get; private set; }

        public virtual DataReturnUploadRawData RawData { get; set; }

        public virtual TimeSpan ProcessTime { get; private set; }

        public virtual DataReturnVersion DataReturnVersion { get; private set; }

        public DataReturnUpload(Scheme scheme, string data, List<DataReturnUploadError> errors, string fileName, int? year, int? quarter)
        {
            Guard.ArgumentNotNull(() => scheme, scheme);           

            Scheme = scheme;
            Errors = errors;
            RawData = new DataReturnUploadRawData() { Data = data };
            this.Date = SystemTime.UtcNow;
            FileName = fileName;            
            ComplianceYear = year;
            Quarter = quarter;
        }
        
        protected DataReturnUpload()
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

        public virtual void SetDataReturnVersion(DataReturnVersion returnVersion)
        {
            Guard.ArgumentNotNull(() => returnVersion, returnVersion);
            DataReturnVersion = returnVersion;
        }
        public void Submit(string userId)
        {
            if (DataReturnVersion != null)
            {
                DataReturnVersion.Submit(userId);
            }
            else
            {
                string errorMessage = "This data return version not defined.";
                throw new InvalidOperationException(errorMessage);
            }          
        }
    }
}