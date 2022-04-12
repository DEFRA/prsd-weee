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

            var aatfAddressDataList = new List<ReturnAatfAddressResult>();
            var aatfAddresses = await getSearchAnAatfAddressDataAccess.GetSearchAnAatfAddressBySearchTerm(message);

            foreach (var item in aatfAddresses)
            {
                var returnAatfAddressData = new ReturnAatfAddressResult()
                {
                    SearchTermId = item.Id,
                    SearchTermName = item.Name
                };

                aatfAddressDataList.Add(returnAatfAddressData);
            }            

            return aatfAddressDataList;
        }
    }
}
