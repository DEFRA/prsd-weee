namespace EA.Weee.Requests.Scheme.MemberRegistration
{
    using Core.Shared;
    using Prsd.Core.Mediator;
    using System;
    using System.Collections.Generic;

    public class GetMemberUploadData : IRequest<List<MemberUploadErrorData>>
    {
        public Guid PcsId { get; private set; }

        public Guid MemberUploadId { get; private set; }

        public GetMemberUploadData(Guid pcsId, Guid memberUploadId)
        {
            PcsId = pcsId;
            MemberUploadId = memberUploadId;
        }
    }
}
