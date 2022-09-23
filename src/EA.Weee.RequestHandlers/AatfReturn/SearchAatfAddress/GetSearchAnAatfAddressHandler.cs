namespace EA.Weee.RequestHandlers.AatfReturn.SearchAatfAddress
{    
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class GetSearchAnAatfAddressHandler : IRequestHandler<GetSearchAatfAddress, List<ReturnAatfAddressResult>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly ISearchAnAatfAddressDataAccess getSearchAnAatfAddressDataAccess;
        
        public GetSearchAnAatfAddressHandler(IWeeeAuthorization authorization, ISearchAnAatfAddressDataAccess getSearchAnAatfAddressDataAccess)
        {
            this.authorization = authorization;
            this.getSearchAnAatfAddressDataAccess = getSearchAnAatfAddressDataAccess;            
        }

        public async Task<List<ReturnAatfAddressResult>> HandleAsync(GetSearchAatfAddress message)
        {
            authorization.EnsureCanAccessExternalArea();

            var resultsAATFAddresses = await getSearchAnAatfAddressDataAccess.GetSearchAnAatfAddressBySearchTerm(message);

            return resultsAATFAddresses;
        }
    }
}
