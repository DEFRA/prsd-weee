namespace EA.Weee.RequestHandlers.Admin.Aatf
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

    public class GetAatfsHandler : IRequestHandler<GetAatfs, List<AatfData>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGetAatfsDataAccess dataAccess;
        private readonly IMap<Aatf, AatfData> aatfmap;

        public GetAatfsHandler(IWeeeAuthorization authorization, IMap<Aatf, AatfData> map, IGetAatfsDataAccess dataAccess)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
            this.aatfmap = map;
        }
        public async Task<List<AatfData>> HandleAsync(GetAatfs message)
        {
           authorization.EnsureCanAccessInternalArea();
           
           List<Aatf> aatfs = await dataAccess.GetAatfs();

            return aatfs
                .OrderBy(a => a.Name)
                .Select(s => aatfmap.Map(s))
                .ToList();
        }
    }
}
