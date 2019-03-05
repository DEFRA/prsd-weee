namespace EA.Weee.RequestHandlers.AatfReturn.ObligatedReused
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.ObligatedGeneric;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.Obligated;

    internal class GetAatfSiteRequestHandler : IRequestHandler<GetAatfSite, List<AddressData>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGetAatfSiteDataAccess getAatfSiteDataAccess;
        private readonly IMap<AatfAddress, AddressData> mapper;

        public GetAatfSiteRequestHandler(IWeeeAuthorization authorization, IGetAatfSiteDataAccess getAatfSiteDataAccess, IMap<AatfAddress, AddressData> mapper)
        {
            this.authorization = authorization;
            this.getAatfSiteDataAccess = getAatfSiteDataAccess;
            this.mapper = mapper;
        }

        public async Task<List<AddressData>> HandleAsync(GetAatfSite message)
        {
            authorization.EnsureCanAccessExternalArea();

            var data = await getAatfSiteDataAccess.GetAddresses(message.AatfId, message.ReturnId);

            var result = new List<AddressData>();

            foreach (var address in data)
            {
                result.Add(mapper.Map(address));
            }

            return result;
        }
    }
}
