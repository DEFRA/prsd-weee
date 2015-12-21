namespace EA.Weee.Domain.Scheme
{
    using System;
    using System.Collections.Generic;
    using Audit;
    using Events;
    using Organisation;
    using Producer;
    using Prsd.Core;

    public class MemberUpload : AuditableEntity
    {
        public virtual Guid OrganisationId { get; private set; }

        public virtual Organisation Organisation { get; private set; }

        public virtual Scheme Scheme { get; private set; }

        public virtual List<MemberUploadError> Errors { get; private set; }

        public virtual int? ComplianceYear { get; private set; }

        public virtual bool IsSubmitted { get; private set; }

        public virtual List<ProducerSubmission> ProducerSubmissions { get; private set; }

        public decimal TotalCharges { get; private set; }

        public virtual string FileName { get; private set; }

        public virtual TimeSpan ProcessTime { get; private set; }

        public virtual MemberUploadRawData RawData { get; set; }

        public MemberUpload(
            Guid organisationId,
            string data,
            List<MemberUploadError> errors,
            decimal totalCharges,
            int? complianceYear,
            Scheme scheme,
            string fileName,
            string userId = null)
        {
            Guard.ArgumentNotNull(() => scheme, scheme);

            OrganisationId = organisationId;
            Scheme = scheme;
            Errors = errors;
            IsSubmitted = false;
            TotalCharges = totalCharges;
            ComplianceYear = complianceYear;
            RawData = new MemberUploadRawData() { Data = data };
            UserId = userId;
            FileName = fileName;
            ProducerSubmissions = new List<ProducerSubmission>();
        }

        public MemberUpload(
            Guid organisationId,
            Scheme scheme,
            string data,
            string fileName)
        {
            Guard.ArgumentNotNull(() => scheme, scheme);

            OrganisationId = organisationId;
            Scheme = scheme;
            Errors = new List<MemberUploadError>();
            RawData = new MemberUploadRawData() { Data = data };
            FileName = fileName;
            ProducerSubmissions = new List<ProducerSubmission>();
        }

        public void Submit()
        {
            if (IsSubmitted)
            {
                throw new InvalidOperationException("IsSubmitted status must be false to transition to true");
            }

            IsSubmitted = true;

            foreach (ProducerSubmission producerSubmission in ProducerSubmissions)
            {
                producerSubmission.RegisteredProducer.SetCurrentSubmission(producerSubmission);
            }
        }

        public void DeductFromTotalCharges(decimal amount)
        {
            TotalCharges -= amount;
        }

        protected MemberUpload()
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
                throw new InvalidOperationException("ProcessTime cannot be set for a MemberUpload that has already been given a ProcessTime value.");
            }
        }
    }
}