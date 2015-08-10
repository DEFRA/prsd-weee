namespace EA.Weee.Requests.PCS.MemberRegistration
{
    using System;
    using Core.PCS;
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
