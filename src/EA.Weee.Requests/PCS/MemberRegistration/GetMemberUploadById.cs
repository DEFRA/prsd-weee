namespace EA.Weee.Requests.PCS.MemberRegistration
{
    using System;
    using Core.PCS;
    using Prsd.Core.Mediator;

    public class GetMemberUploadById : IRequest<MemberUploadData>
    {
        public Guid MemberUploadId { get; private set; }

        public GetMemberUploadById(Guid memberUploadId)
        {
            MemberUploadId = memberUploadId;
        }
    }
}
