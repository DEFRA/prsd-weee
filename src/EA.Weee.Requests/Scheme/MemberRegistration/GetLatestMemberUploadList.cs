namespace EA.Weee.Requests.Scheme.MemberRegistration
{
    using System;
    using Core.Scheme;
    using Prsd.Core.Mediator;

    public class GetLatestMemberUploadList : IRequest<LatestMemberUploadList>
    {
        public Guid PcsId { get; private set; }

        public GetLatestMemberUploadList(Guid pcsId)
        {
            PcsId = pcsId;
        }
    }
}
