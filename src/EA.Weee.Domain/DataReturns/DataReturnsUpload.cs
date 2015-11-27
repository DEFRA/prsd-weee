namespace EA.Weee.Domain.Scheme
{
    using Prsd.Core.Domain;
    using System;
    using System.Collections.Generic;

    public class DataReturnsUpload : Entity
    {
        public virtual Guid SchemeId { get; private set; }
             
        public virtual Scheme Scheme { get; private set; }

        public virtual List<DataReturnsUploadError> Errors { get; private set; }

        public virtual int? ComplianceYear { get; private set; }

        public virtual bool IsSubmitted { get; private set; }

        public virtual string FileName { get; private set; }

        public virtual TimeSpan ProcessTime { get; private set; }

        public virtual DataReturnsUploadRawData RawData { get; set; }

        public DataReturnsUpload(string data, List<DataReturnsUploadError> errors, int? complianceYear, Guid schemeId, string fileName)
        {
            SchemeId = schemeId;
            Errors = errors;
            IsSubmitted = false;
            ComplianceYear = complianceYear;
            RawData = new DataReturnsUploadRawData() { Data = data };
        }
        public DataReturnsUpload(Guid organisationId, Guid schemeId, string data, string fileName)
        {
            SchemeId = schemeId;
            Errors = new List<DataReturnsUploadError>();
            RawData = new DataReturnsUploadRawData { Data = data };
            FileName = fileName;
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
        }

        public virtual void SetProcessTime(TimeSpan processTime)
        {
            if (ProcessTime.Ticks.Equals(0))
            {
                ProcessTime = processTime;
            }
            else
        {
                throw new InvalidOperationException("ProcessTime cannot be set for a upload that has already been given a ProcessTime value.");
            }
        }
    }
}