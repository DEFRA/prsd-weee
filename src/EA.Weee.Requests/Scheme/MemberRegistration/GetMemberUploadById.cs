namespace EA.Weee.Requests.Scheme.MemberRegistration
{
    using System;
    using Core.Scheme;
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
