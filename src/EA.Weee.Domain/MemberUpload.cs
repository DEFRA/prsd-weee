namespace EA.Weee.Domain
{
    using System;
    using System.Collections.Generic;
    using EA.Prsd.Core.Domain;

    public class MemberUpload : Entity
    {
        public Guid OrganisationId { get; private set; }
        public virtual Organisation Organisation { get; private set; }

        public string Data { get; private set; }

        public virtual List<MemberUploadError> Errors { get; private set; }

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