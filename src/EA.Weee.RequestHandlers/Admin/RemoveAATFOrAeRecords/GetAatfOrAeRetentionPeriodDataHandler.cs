namespace EA.Weee.RequestHandlers.Admin.RemoveAATFOrAeRecords
{
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Admin.RemoveAATFOrAeRecords;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class GetAatfOrAeRetentionPeriodDataHandler : IRequestHandler<GetAatfOrAeRetentionPeriodData, List<AatfDataList>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGetAatfOrAeRetentionPeriodDataAccess dataAccess;
        private readonly IMap<Aatf, AatfDataList> aatfmap;

        public GetAatfOrAeRetentionPeriodDataHandler(IWeeeAuthorization authorization, IMap<Aatf, AatfDataList> map,
                                                     IGetAatfOrAeRetentionPeriodDataAccess dataAccess)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
            this.aatfmap = map;
        }

        public async Task<List<AatfDataList>> HandleAsync(GetAatfOrAeRetentionPeriodData message)
        {
            authorization.EnsureCanAccessInternalArea();

            var aatfs = await dataAccess.GetFilteredAatfs(message.Filter);

            return aatfs.Select(s => aatfmap.Map(s)).ToList();
        }
    }
}
