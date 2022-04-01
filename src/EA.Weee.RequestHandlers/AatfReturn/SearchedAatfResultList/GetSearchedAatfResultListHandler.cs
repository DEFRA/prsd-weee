namespace EA.Weee.RequestHandlers.AatfReturn.SearchedAatfResultList
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class GetSearchedAatfResultListHandler : IRequestHandler<GetAatfAddressBySearchId, List<WeeeSearchedAnAatfListData>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly ISearchedAatfResultListDataAccess searchedAatfResultListDataAccess;

        public GetSearchedAatfResultListHandler(IWeeeAuthorization authorization, ISearchedAatfResultListDataAccess searchedAatfResultListDataAccess)
        {
            this.authorization = authorization;
            this.searchedAatfResultListDataAccess = searchedAatfResultListDataAccess;
        }

        public async Task<List<WeeeSearchedAnAatfListData>> HandleAsync(GetAatfAddressBySearchId model)
        {
            authorization.EnsureCanAccessExternalArea();
            var aatfAddressDataList = await searchedAatfResultListDataAccess.GetAnAatfBySearchId(model.SearchedAatfId);

            return aatfAddressDataList;
        }
    }
}
