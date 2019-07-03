namespace EA.Weee.RequestHandlers.Admin.GetAatfs
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AatfReports;
    using Core.Admin;
    using Core.Admin.AatfReports;
    using DataAccess;
    using DataAccess.StoredProcedure;
    using Domain.Admin.AatfReports;
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Admin;
    using Requests.Admin.Aatf;
    using FacilityType = Core.AatfReturn.FacilityType;

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

            return submissionHistoryItems
                .OrderByDescending(s => s.ComplianceYear)
                .ThenByDescending(s => s.Quarter)
                .ThenByDescending(s => s.SubmittedDate)
                .Select(s => mapper.Map<AatfSubmissionHistory, AatfSubmissionHistoryData>(s)).ToList();
        }
    }
}
