namespace EA.Weee.RequestHandlers.Admin.GetAatfs
{
    using AatfReports;
    using Core.Admin.AatfReports;
    using Domain.Admin.AatfReports;
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.RequestHandlers.Security;
    using Requests.Admin.Aatf;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class GetAatfSubmissionHistoryHandler : IRequestHandler<GetAatfSubmissionHistory, List<AatfSubmissionHistoryData>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGetAatfSubmissionHistoryDataAccess getAatfSubmissionHistoryDataAccess;
        private readonly IMapper mapper;

        public GetAatfSubmissionHistoryHandler(IGetAatfSubmissionHistoryDataAccess getAatfSubmissionHistoryDataAccess, IWeeeAuthorization authorization, IMapper mapper)
        {
            this.getAatfSubmissionHistoryDataAccess = getAatfSubmissionHistoryDataAccess;
            this.authorization = authorization;
            this.mapper = mapper;
        }

        public async Task<List<AatfSubmissionHistoryData>> HandleAsync(GetAatfSubmissionHistory message)
        {
            authorization.EnsureCanAccessInternalArea();

            var submissionHistoryItems = await getAatfSubmissionHistoryDataAccess.GetItemsAsync(message.AatfId);

            return submissionHistoryItems.Select(s => mapper.Map<AatfSubmissionHistory, AatfSubmissionHistoryData>(s)).OrderByDescending(s => s.ComplianceYear)
                .ThenByDescending(s => s.Quarter)
                .ThenByDescending(s => s.SubmittedDate)
                .ToList();
        }
    }
}
