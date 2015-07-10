namespace EA.Weee.Domain.PCS
{
    using System;
    using System.Collections.Generic;
    using EA.Prsd.Core.Domain;
    using Organisation;

    public class MemberUpload : Entity
    {
        //TODO: Remove Organisation from here as it's property of Scheme
        public Guid OrganisationId { get; private set; }
        public virtual Organisation Organisation { get; private set; }

        public string Data { get; private set; }

        public int ComplianceYear { get; private set; }

        public virtual List<MemberUploadError> Errors { get; private set; }

        public virtual Scheme Scheme { get; private set; }

        public bool IsSubmitted { get; private set; }

        public MemberUpload(Guid organisationId, string data, List<MemberUploadError> errors)
        {
            OrganisationId = organisationId;
            Data = data;
            Errors = errors;
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
    }
}