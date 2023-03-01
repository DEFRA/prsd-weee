namespace EA.Weee.RequestHandlers.Aatf
{
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.AatfTaskList;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Aatf;
    using System.Threading.Tasks;

    public class GetAatfByIdHandler : IRequestHandler<GetAatfById, AatfData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IFetchAatfDataAccess fetchDataAccess;
        private readonly IMap<Aatf, AatfData> mapper;

        public GetAatfByIdHandler(IWeeeAuthorization authorization, IMap<Aatf, AatfData> mapper, IFetchAatfDataAccess fetchDataAccess)
        {
            this.authorization = authorization;
            this.mapper = mapper;
            this.fetchDataAccess = fetchDataAccess;
        }

        public async Task<AatfData> HandleAsync(GetAatfById message)
        {
            authorization.EnsureCanAccessExternalArea();

            var aatf = await fetchDataAccess.FetchById(message.AatfId);

            return mapper.Map(aatf);
        }
    }
}
