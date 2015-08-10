namespace EA.Weee.Requests.Scheme.MemberRegistration
{
    using System;
    using Core.Scheme;
    using Prsd.Core.Mediator;

    public class GetProducerCSVByMemberUploadId : IRequest<ProducerCSVFileData>
    {
        public Guid MemberUploadId { get; private set; }

        public GetProducerCSVByMemberUploadId(Guid memberUploadId)
        {
            MemberUploadId = memberUploadId;
        }
    }
}
