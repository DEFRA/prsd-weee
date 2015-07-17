namespace EA.Weee.Domain.PCS
{
    using System;
    using System.Collections.Generic;
    using EA.Prsd.Core.Domain;
    using Organisation;
    using Producer;
    using Prsd.Core;

    public class MemberUpload : Entity
    {
        public Guid OrganisationId { get; private set; }

        public Guid? SchemeId { get; private set; }
        
        public virtual Organisation Organisation { get; private set; }

        public virtual Scheme Scheme { get; private set; }

        public virtual List<MemberUploadError> Errors { get; private set; }

        public string Data { get; private set; }

        public int ComplianceYear { get; private set; }

        public bool IsSubmitted { get; private set; }

        public virtual List<Producer> Producers { get; private set; }

        public MemberUpload(Guid organisationId, string data, List<MemberUploadError> errors, Guid? schemeId = null)
        {
            OrganisationId = organisationId;
            SchemeId = schemeId.GetValueOrDefault();
            Data = data;
            Errors = errors;
            IsSubmitted = false;
            //TODO: Needs doing properly
            ComplianceYear = SystemTime.UtcNow.Year + 1;
        }

        public MemberUpload(Guid organisationId, string data)
        {
            OrganisationId = organisationId;
            Data = data;
            Errors = new List<MemberUploadError>();
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