namespace EA.Weee.RequestHandlers.AatfReturn.ObligatedSentOn
{
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.DataAccess;
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
        private readonly IWeeeSentOnDataAccess getSentOnAatfSiteDataAccess;
        private readonly IFetchObligatedWeeeForReturnDataAccess fetchWeeeSentOnAmountDataAccess;
        private readonly IMap<AatfAddress, AatfAddressData> addressMapper;
        private readonly WeeeContext context;

        public GetWeeeSentOnHandler(IWeeeAuthorization authorization,
            IWeeeSentOnDataAccess getSentOnAatfSiteDataAccess,
            IFetchObligatedWeeeForReturnDataAccess fetchWeeeSentOnAmountDataAccess,
            IMap<AatfAddress, AatfAddressData> addressMapper,
            WeeeContext context)
        {
            this.authorization = authorization;
            this.getSentOnAatfSiteDataAccess = getSentOnAatfSiteDataAccess;
            this.fetchWeeeSentOnAmountDataAccess = fetchWeeeSentOnAmountDataAccess;
            this.addressMapper = addressMapper;
            this.context = context;
        }

        public async Task<List<WeeeSentOnData>> HandleAsync(GetWeeeSentOn message)
        {
            authorization.EnsureCanAccessExternalArea();

            var weeeSentOnList = new List<WeeeSentOnData>();
            var weeeSentOn = await getSentOnAatfSiteDataAccess.GetWeeeSentOnByReturnAndAatf(message.AatfId, message.ReturnId);

            foreach (var item in weeeSentOn)
            {
                var amount = await fetchWeeeSentOnAmountDataAccess.FetchObligatedWeeeSentOnForReturn(item.Id);

                var weeeSentOnObligatedData = amount.Select(n => new WeeeObligatedData(n.Id, new AatfData(n.WeeeSentOn.Aatf.Id, n.WeeeSentOn.Aatf.Name, n.WeeeSentOn.Aatf.ApprovalNumber, n.WeeeSentOn.Aatf.ComplianceYear), n.CategoryId, n.NonHouseholdTonnage, n.HouseholdTonnage)).ToList();

                var weeeSentOnData = new WeeeSentOnData()
                {
                    SiteAddress = addressMapper.Map(item.SiteAddress),
                    Tonnages = weeeSentOnObligatedData,
                    WeeeSentOnId = item.Id,
                    SiteAddressId = item.SiteAddress.Id
                };

                if (item.OperatorAddress != null)
                {
                    weeeSentOnData.OperatorAddress = addressMapper.Map(item.OperatorAddress);
                    weeeSentOnData.OperatorAddressId = item.OperatorAddress.Id;
                }
                else
                {
                    var aatfData = context.Aatfs.Where(x => x.SiteAddressId == item.SiteAddressId && x.Organisation.BusinessAddressId == item.OperatorAddressId).SingleOrDefault();
                    var country = context.Countries.Where(x => x.Id == aatfData.Organisation.BusinessAddress.CountryId).SingleOrDefault();

                    weeeSentOnData.OperatorAddress = new AatfAddressData()
                    {
                        Name = aatfData.Organisation.Name,
                        Address1 = aatfData.Organisation.BusinessAddress.Address1,
                        Address2 = aatfData.Organisation.BusinessAddress.Address2,
                        TownOrCity = aatfData.Organisation.BusinessAddress.TownOrCity,
                        Postcode = aatfData.Organisation.BusinessAddress.Postcode,
                        CountryId = aatfData.Organisation.BusinessAddress.CountryId,
                        CountyOrRegion = aatfData.Organisation.BusinessAddress.CountyOrRegion,
                        CountryName = country.Name
                    };
                    weeeSentOnData.OperatorAddressId = item.OperatorAddressId;
                    weeeSentOnData.ApprovalNumber = aatfData.ApprovalNumber;
                }

                weeeSentOnList.Add(weeeSentOnData);
            }

            if (message.WeeeSentOnId != null)
            {
                var weeeSentOnListFiltered = new List<WeeeSentOnData>();
                var weeeSentOnSelected = weeeSentOnList.Where(w => w.WeeeSentOnId == message.WeeeSentOnId).Select(w => w).SingleOrDefault();
                weeeSentOnListFiltered.Add(weeeSentOnSelected);
                return weeeSentOnListFiltered;
            }

            return weeeSentOnList;
        }
    }
}
