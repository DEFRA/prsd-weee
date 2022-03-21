namespace EA.Weee.RequestHandlers.AatfReturn.SearchAatfAddress
{
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.AatfTaskList;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class GetSearchAnAatfAddressHandler : IRequestHandler<GetSearchAatfAddress, List<ReturnAatfAddressData>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly ISearchAnAatfAddressDataAccess getSearchAnAatfAddressDataAccess;
        private readonly IFetchObligatedWeeeForReturnDataAccess fetchWeeeSentOnAmountDataAccess;
        private readonly IMap<AatfAddress, ReturnAatfAddressData> addressMapper;

        public GetSearchAnAatfAddressHandler(IWeeeAuthorization authorization, 
                                             ISearchAnAatfAddressDataAccess getSearchAnAatfAddressDataAccess, 
                                             IFetchObligatedWeeeForReturnDataAccess fetchWeeeSentOnAmountDataAccess)
        {
            this.authorization = authorization;
            this.getSearchAnAatfAddressDataAccess = getSearchAnAatfAddressDataAccess;
            this.fetchWeeeSentOnAmountDataAccess = fetchWeeeSentOnAmountDataAccess;
        }

        public async Task<List<ReturnAatfAddressData>> HandleAsync(GetSearchAatfAddress message)
        {
            authorization.EnsureCanAccessExternalArea();

            var aatfAddressDataList = new List<ReturnAatfAddressData>();
            var aatfAddresses = await getSearchAnAatfAddressDataAccess.GetSearchAnAatfAddressBySearchTerm(message.SearchTerm);

            foreach (var item in aatfAddresses)
            {
                var returnAatfAddressData = new ReturnAatfAddressData()
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
