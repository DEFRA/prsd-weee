namespace EA.Weee.Requests.PCS.MemberRegistration
{
    using System;
    using Core.PCS;
    using Prsd.Core.Mediator;

    public class GetLatestMemberUploadSummary : IRequest<LatestMemberUploadSummary>
    {
        public Guid PcsId { get; private set; }

        public GetLatestMemberUploadSummary(Guid pcsId)
        {
            PcsId = pcsId;
        }
    }
}
