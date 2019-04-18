namespace EA.Weee.RequestHandlers.AatfReturn.ObligatedSentOn
{
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.AatfTaskList;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class GetWeeeSentOnHandler : IRequestHandler<GetWeeeSentOn, List<WeeeSentOnData>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly ISentOnAatfSiteDataAccess getSentOnAatfSiteDataAccess;
        private readonly IFetchObligatedWeeeForReturnDataAccess fetchWeeeSentOnAmountDataAccess;
        private readonly IMap<AatfAddress, AatfAddressData> addressMapper;

        public GetWeeeSentOnHandler(IWeeeAuthorization authorization,
            ISentOnAatfSiteDataAccess getSentOnAatfSiteDataAccess, IFetchObligatedWeeeForReturnDataAccess fetchWeeeSentOnAmountDataAccess, IMap<AatfAddress, AatfAddressData> addressMapper)
        {
            this.authorization = authorization;
            this.getSentOnAatfSiteDataAccess = getSentOnAatfSiteDataAccess;
            this.fetchWeeeSentOnAmountDataAccess = fetchWeeeSentOnAmountDataAccess;
            this.addressMapper = addressMapper;
        }

        public async Task<List<WeeeSentOnData>> HandleAsync(GetWeeeSentOn message)
        {
            authorization.EnsureCanAccessExternalArea();

            var weeeSentOnList = new List<WeeeSentOnData>();
            var weeeSentOn = await getSentOnAatfSiteDataAccess.GetWeeeSentOnByReturnAndAatf(message.AatfId, message.ReturnId);

            foreach (var item in weeeSentOn)
            {
                var amount = await fetchWeeeSentOnAmountDataAccess.FetchObligatedWeeeSentOnForReturn(item.Id);

                var weeeSentOnObligatedData = amount.Select(n => new WeeeObligatedData(n.Id, new AatfData(n.WeeeSentOn.Aatf.Id, n.WeeeSentOn.Aatf.Name, n.WeeeSentOn.Aatf.ApprovalNumber), n.CategoryId, n.NonHouseholdTonnage, n.HouseholdTonnage)).ToList();

                var weeeSentOnData = new WeeeSentOnData()
                {
                    OperatorAddress = addressMapper.Map(item.OperatorAddress),
                    SiteAddress = addressMapper.Map(item.SiteAddress),
                    Tonnages = weeeSentOnObligatedData,
                    WeeeSentOnId = item.Id
                };

                weeeSentOnList.Add(weeeSentOnData);
            }

            return weeeSentOnList;
        }
    }
}
