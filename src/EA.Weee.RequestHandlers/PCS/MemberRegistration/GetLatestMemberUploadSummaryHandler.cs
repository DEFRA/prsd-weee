namespace EA.Weee.RequestHandlers.PCS.MemberRegistration
{
    using System;
    using System.Threading.Tasks;
    using Core.PCS;
    using Prsd.Core.Mediator;
    using Requests.PCS.MemberRegistration;

    public class GetLatestMemberUploadSummaryHandler : IRequestHandler<GetLatestMemberUploadSummary, LatestMemberUploadSummary>
    {
        public async Task<LatestMemberUploadSummary> HandleAsync(GetLatestMemberUploadSummary message)
        {
            // TODO:
            return await Task.Run(() => new LatestMemberUploadSummary
            {
                MemberUploadId = Guid.NewGuid()
            });
        }
    }
}
