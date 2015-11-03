namespace EA.Weee.Domain.Scheme
{
    using System;
    using System.Collections.Generic;
    using Audit;
    using Events;
    using Organisation;
    using Producer;

    public class MemberUpload : AuditableEntity
    {
        public virtual Guid OrganisationId { get; private set; }

        public virtual Guid SchemeId { get; private set; }

        public virtual Organisation Organisation { get; private set; }

        public virtual Scheme Scheme { get; private set; }

        public virtual List<MemberUploadError> Errors { get; private set; }

        public virtual int? ComplianceYear { get; private set; }

        public virtual bool IsSubmitted { get; private set; }

        public virtual List<Producer> Producers { get; private set; }

        public decimal TotalCharges { get; private set; }

        public virtual MemberUploadRawData RawData { get; set; }

        public MemberUpload(Guid organisationId, string data, List<MemberUploadError> errors, decimal totalCharges,
            int? complianceYear, Guid schemeId, string userId = null)
        {
            OrganisationId = organisationId;
            SchemeId = schemeId;
            Errors = errors;
            IsSubmitted = false;
            TotalCharges = totalCharges;
            ComplianceYear = complianceYear;
            RawData = new MemberUploadRawData() { Data = data };
            UserId = userId;
        }

        public MemberUpload(Guid organisationId, Guid schemeId, string data)
        {
            OrganisationId = organisationId;
            SchemeId = schemeId;
            Errors = new List<MemberUploadError>();
            RawData = new MemberUploadRawData() { Data = data };
        }

        public void Submit()
        {
            if (IsSubmitted)
            {
                throw new InvalidOperationException("IsSubmitted status must be false to transition to true");
            }

            IsSubmitted = true;

            RaiseEvent(new MemberUploadSubmittedEvent(this));
        }

        protected MemberUpload()
        {
        }

        public void SetProducers(List<Producer> producers)
        {
            Producers = producers;
        }
    }
}