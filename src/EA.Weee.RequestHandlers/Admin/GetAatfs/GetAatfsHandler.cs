namespace EA.Weee.RequestHandlers.Admin.GetAatfs
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Admin;
    using FacilityType = Core.AatfReturn.FacilityType;

    public class GetAatfsHandler : IRequestHandler<GetAatfs, List<AatfDataList>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGetAatfsDataAccess dataAccess;
        private readonly IMap<Aatf, AatfDataList> aatfmap;

        public GetAatfsHandler(IWeeeAuthorization authorization, IMap<Aatf, AatfDataList> map, IGetAatfsDataAccess dataAccess)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
            this.aatfmap = map;
        }
        public async Task<List<AatfDataList>> HandleAsync(GetAatfs message)
        {
            authorization.EnsureCanAccessInternalArea();

            List<Aatf> aatfs = await dataAccess.GetAatfs();

            switch (message.FacilityType)
            {
                case FacilityType.Aatf:
                    return SortAatfs(aatfs, FacilityType.Aatf);
                case FacilityType.Ae:
                    return SortAatfs(aatfs, FacilityType.Ae);
                default:
                    return aatfs.OrderBy(a => a.Name).Select(s => aatfmap.Map(s)).ToList();
            }
        }

        private List<Core.AatfReturn.AatfDataList> SortAatfs(List<Aatf> aatfs, FacilityType facilityType)
        {
            return aatfs.OrderBy(a => a.Name).Where(w => w.FacilityType.Value == (int)facilityType).Select(s => aatfmap.Map(s)).ToList();
        }
    }
}
