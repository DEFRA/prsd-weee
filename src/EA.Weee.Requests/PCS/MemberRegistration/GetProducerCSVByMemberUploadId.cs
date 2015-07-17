namespace EA.Weee.Requests.PCS.MemberRegistration
{
    using System;
    using Prsd.Core.Mediator;

    public class GetProducerCSVByMemberUploadId : IRequest<string>
    {
        public Guid MemberUploadId { get; private set; }

        public GetProducerCSVByMemberUploadId(Guid memberUploadId)
        {
            MemberUploadId = memberUploadId;
        }
    }
}
