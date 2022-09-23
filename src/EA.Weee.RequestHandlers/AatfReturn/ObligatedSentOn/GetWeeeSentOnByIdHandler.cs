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

    public class GetWeeeSentOnByIdHandler : IRequestHandler<GetWeeeSentOnById, WeeeSentOnData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IWeeeSentOnDataAccess getSentOnAatfSiteDataAccess;
        private readonly IFetchObligatedWeeeForReturnDataAccess fetchWeeeSentOnAmountDataAccess;
        private readonly IMap<AatfAddress, AatfAddressData> addressMapper;
        private readonly WeeeContext context;

        public GetWeeeSentOnByIdHandler(IWeeeAuthorization authorization,
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

            var weeeSentOnObligatedData = amount.Select(n => new WeeeObligatedData(n.Id, new AatfData(n.WeeeSentOn.Aatf.Id, n.WeeeSentOn.Aatf.Name, n.WeeeSentOn.Aatf.ApprovalNumber, n.WeeeSentOn.Aatf.ComplianceYear), n.CategoryId, n.NonHouseholdTonnage, n.HouseholdTonnage)).ToList();

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
            else
            {
                var aatfData = context.Aatfs.Where(x => x.SiteAddressId == weeeSentOn.SiteAddressId && x.Organisation.BusinessAddressId == weeeSentOn.OperatorAddressId).SingleOrDefault();
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
            }

            return weeeSentOnData;
        }
    }
}
