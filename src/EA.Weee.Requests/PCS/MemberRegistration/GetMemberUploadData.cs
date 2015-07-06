namespace EA.Weee.Requests.PCS.MemberRegistration
{
    using System;
    using System.Collections.Generic;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Requests.Shared;

    public class GetMemberUploadData : IRequest<List<MemberUploadErrorData>>
    {
        public Guid MemberUploadId { get; private set; }

        public GetMemberUploadData(Guid memberUploadId)
        {
            MemberUploadId = memberUploadId;
        }
    }
}
