namespace EA.Weee.Requests.Scheme.MemberRegistration
{
    using System;
    using Core.Scheme;
    using Prsd.Core.Mediator;

    public class GetProducerCSVByMemberUploadId : IRequest<ProducerCSVFileData>
    {
        public Guid OrganisationId { get; private set; }
        public Guid MemberUploadId { get; private set; }

        public GetProducerCSVByMemberUploadId(Guid organisationId, Guid memberUploadId)
        {
            OrganisationId = organisationId;
            MemberUploadId = memberUploadId;
        }
    }
}
