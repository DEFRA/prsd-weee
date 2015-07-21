namespace EA.Weee.Requests.PCS.MemberRegistration
{
    using System;
    using Core.PCS;
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
