﻿namespace EA.Weee.RequestHandlers.Aatf
{
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.AatfTaskList;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Aatf;
    using System.Threading.Tasks;

    public class GetAatfByIdExternalHandler : IRequestHandler<GetAatfByIdExternal, AatfData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IFetchAatfDataAccess fetchDataAccess;
        private readonly IMap<Aatf, AatfData> mapper;

        public GetAatfByIdExternalHandler(IWeeeAuthorization authorization, IMap<Aatf, AatfData> mapper, IFetchAatfDataAccess fetchDataAccess)
        {
            this.authorization = authorization;
            this.mapper = mapper;
            this.fetchDataAccess = fetchDataAccess;
        }

        public async Task<AatfData> HandleAsync(GetAatfByIdExternal message)
        {
            authorization.EnsureCanAccessExternalArea();

            var aatf = await fetchDataAccess.FetchById(message.AatfId);

            authorization.EnsureOrganisationAccess(aatf.Organisation.Id);

            return mapper.Map(aatf);
        }
    }
}
