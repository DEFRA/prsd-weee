namespace EA.Weee.RequestHandlers.Aatf
{
    using EA.Prsd.Core.Domain;
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Aatf;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.AatfTaskList;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Aatf;
    using System.Threading.Tasks;

    public class GetAatfByIdExternalHandler : IRequestHandler<GetAatfByIdExternal, AatfDataExternal>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IUserContext context;
        private readonly IFetchAatfDataAccess fetchDataAccess;
        private readonly IMap<AatfContact, AatfContactData> mapper;

        public GetAatfByIdExternalHandler(IWeeeAuthorization authorization, IUserContext context, IMap<AatfContact, AatfContactData> mapper, IFetchAatfDataAccess fetchDataAccess)
        {
            this.authorization = authorization;
            this.context = context;
            this.mapper = mapper;
            this.fetchDataAccess = fetchDataAccess;
        }

        public async Task<AatfDataExternal> HandleAsync(GetAatfByIdExternal message)
        {
            authorization.EnsureCanAccessExternalArea();

            var aatf = await fetchDataAccess.FetchById(message.AatfId);

            var aatfContactData = mapper.Map(aatf.Contact);

            var aatfData = new AatfDataExternal(aatf.Id, aatf.Name)
            {
                Contact = aatfContactData,
                ApprovalNumber = aatf.ApprovalNumber,
                FacilityType = aatf.FacilityType.DisplayName
            };

            return aatfData;
        }
    }
}
