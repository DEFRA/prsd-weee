namespace EA.Weee.RequestHandlers.AatfReturn.SearchedAatfResultList
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.DataAccess;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    public class SearchedAatfResultListDataAccess : ISearchedAatfResultListDataAccess
    {
        private readonly WeeeContext context;

        public SearchedAatfResultListDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<List<WeeeSearchedAnAatfListData>> GetAnAatfBySearchId(Guid selectedAatfId)
        {
            var sentOnList = await context.WeeeSentOn.Where(w => w.AatfId == selectedAatfId)
                .Include(w => w.OperatorAddress)
                .Include(w => w.Aatf)
                .Include(w => w.SiteAddress)
                .ToListAsync();

            var returnSearchedAatfAddressListDatas = new List<WeeeSearchedAnAatfListData>();
            if (sentOnList != null && sentOnList.Count > 0)
            {
                foreach (var sentOn in sentOnList)
                {
                    var returnSearchedAatfAddressListData = new WeeeSearchedAnAatfListData()
                    {
                        ApprovalNumber = sentOn.Aatf.ApprovalNumber,
                        WeeeSentOnId = sentOn.Id,
                        OperatorAddress = new AatfAddressData(sentOn.OperatorAddress.Name, sentOn.OperatorAddress.Address1, sentOn.OperatorAddress.Address2, sentOn.OperatorAddress.TownOrCity, sentOn.OperatorAddress.CountyOrRegion, sentOn.OperatorAddress.Postcode, sentOn.OperatorAddress.CountryId, sentOn.OperatorAddress.Country.Name),
                        SiteAddress = new AatfAddressData(sentOn.SiteAddress.Name, sentOn.SiteAddress.Address1, sentOn.SiteAddress.Address2, sentOn.SiteAddress.TownOrCity, sentOn.SiteAddress.CountyOrRegion, sentOn.SiteAddress.Postcode, sentOn.SiteAddress.CountryId, sentOn.SiteAddress.Country.Name)                        
                    };
                    returnSearchedAatfAddressListDatas.Add(returnSearchedAatfAddressListData);
                }
            }
            return returnSearchedAatfAddressListDatas;
        }
    }
}
