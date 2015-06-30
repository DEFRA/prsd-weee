namespace EA.Weee.Requests.MemberRegistration
{
    using System;
    using System.Collections.Generic;
    using EA.Prsd.Core.Mediator;

    public class GetMemberUploadData : IRequest<List<KeyValuePair<string, string>>>
    {
        public Guid MemberUploadId { get; private set; }

        public GetMemberUploadData(Guid memberUploadId)
        {
            MemberUploadId = memberUploadId;
        }
    }
}
