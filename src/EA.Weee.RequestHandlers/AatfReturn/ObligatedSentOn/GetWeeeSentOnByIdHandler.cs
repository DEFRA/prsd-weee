namespace EA.Weee.RequestHandlers.AatfReturn.ObligatedSentOn
{
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.AatfTaskList;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class GetWeeeSentOnByIdHandler : IRequestHandler<GetWeeeSentOnById, WeeeSentOnData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IWeeeSentOnDataAccess getSentOnAatfSiteDataAccess;
        private readonly IFetchObligatedWeeeForReturnDataAccess fetchWeeeSentOnAmountDataAccess;
        private readonly IMap<AatfAddress, AatfAddressData> addressMapper;

        public GetWeeeSentOnByIdHandler(IWeeeAuthorization authorization,
            IWeeeSentOnDataAccess getSentOnAatfSiteDataAccess, IFetchObligatedWeeeForReturnDataAccess fetchWeeeSentOnAmountDataAccess, IMap<AatfAddress, AatfAddressData> addressMapper)
        {
            this.authorization = authorization;
            this.getSentOnAatfSiteDataAccess = getSentOnAatfSiteDataAccess;
            this.fetchWeeeSentOnAmountDataAccess = fetchWeeeSentOnAmountDataAccess;
            this.addressMapper = addressMapper;
        }

        public async Task<WeeeSentOnData> HandleAsync(GetWeeeSentOnById message)
        {
            authorization.EnsureCanAccessExternalArea();

            var weeeSentOnList = new List<WeeeSentOnData>();
            var weeeSentOn = await getSentOnAatfSiteDataAccess.GetWeeeSentOnById(message.WeeeSentOnId);

            if (weeeSentOn == null)
            {
                return null;
            }

            var amount = await fetchWeeeSentOnAmountDataAccess.FetchObligatedWeeeSentOnForReturn(weeeSentOn.Id);

            var weeeSentOnObligatedData = amount.Select(n => new WeeeObligatedData(n.Id, new AatfData(n.WeeeSentOn.Aatf.Id, n.WeeeSentOn.Aatf.Name, n.WeeeSentOn.Aatf.ApprovalNumber), n.CategoryId, n.NonHouseholdTonnage, n.HouseholdTonnage)).ToList();

            var weeeSentOnData = new WeeeSentOnData()
            {
                SiteAddress = addressMapper.Map(weeeSentOn.SiteAddress),
                Tonnages = weeeSentOnObligatedData,
                WeeeSentOnId = weeeSentOn.Id,
                SiteAddressId = weeeSentOn.SiteAddress.Id
            };

            if (weeeSentOn.OperatorAddress != null)
            {
                weeeSentOnData.OperatorAddress = addressMapper.Map(weeeSentOn.OperatorAddress);
                weeeSentOnData.OperatorAddressId = weeeSentOn.OperatorAddress.Id;
            }

            return weeeSentOnData;
        }
    }
}
