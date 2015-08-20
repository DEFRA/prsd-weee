namespace EA.Weee.Requests.Scheme.MemberRegistration
{
    using System;
    using Prsd.Core.Mediator;

    public class MemberUploadSubmission : IRequest<Guid>
    {
        public Guid OrganisationId { get; private set; }
        public Guid MemberUploadId { get; private set; }

        public MemberUploadSubmission(Guid organisationId, Guid memberUploadId)
        {
            OrganisationId = organisationId;
            MemberUploadId = memberUploadId;
        }
    }
}
