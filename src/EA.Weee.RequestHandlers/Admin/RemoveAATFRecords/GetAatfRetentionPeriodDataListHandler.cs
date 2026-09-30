namespace EA.Weee.RequestHandlers.Admin.RemoveAATFRecords
{
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Admin.RemoveAATFRecords;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class GetAatfRetentionPeriodDataListHandler : IRequestHandler<GetAatfRetentionPeriodDataList, List<AatfDataList>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGetAatfsRetentionPeriodDataAccess dataAccess;
        private readonly IMap<Aatf, AatfDataList> aatfmap;

        public GetAatfRetentionPeriodDataListHandler(IWeeeAuthorization authorization, IMap<Aatf, AatfDataList> map,
                                                     IGetAatfsRetentionPeriodDataAccess dataAccess)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
            this.aatfmap = map;
        }

        public async Task<List<AatfDataList>> HandleAsync(GetAatfRetentionPeriodDataList message)
        {
            authorization.EnsureCanAccessInternalArea();

            var aatfs = await dataAccess.GetFilteredAatfs(message.Filter);

            return aatfs.Select(s => aatfmap.Map(s)).ToList();
        }
    }
}
