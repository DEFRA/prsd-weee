namespace EA.Weee.RequestHandlers.Admin.GetAatfs
{
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Admin;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class GetAatfsByOperatorIdHandler : IRequestHandler<GetAatfsByOperatorId, List<AatfDataList>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGetAatfsDataAccess dataAccess;
        private readonly IMap<Aatf, AatfDataList> aatfmap;

        public GetAatfsByOperatorIdHandler(IWeeeAuthorization authorization, IMap<Aatf, AatfDataList> map, IGetAatfsDataAccess dataAccess)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
            this.aatfmap = map;
        }
        public async Task<List<AatfDataList>> HandleAsync(GetAatfsByOperatorId message)
        {
            authorization.EnsureCanAccessInternalArea();

            List<Aatf> aatfs = await dataAccess.GetAatfs();

            return aatfs
                .OrderBy(a => a.Name)
                .Select(s => aatfmap.Map(s))
                .Where(s => s.Operator.Id == message.OperatorId)
                .ToList();
        }
    }
}
