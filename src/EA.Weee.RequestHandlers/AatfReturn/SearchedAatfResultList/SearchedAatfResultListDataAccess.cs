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

        public async Task<List<WeeeSearchedAnAatfListData>> GetAnAatfBySearchId(Guid selectedAatfId, string searchedTerm, Guid currentSelectedAatfId)
        {
            var returnSearchedAatfAddressListDatas = new List<WeeeSearchedAnAatfListData>();

            if (selectedAatfId != null)
            {
                var sentOnList = await context.Aatfs.Where(w => w.Id == selectedAatfId)
                                                    .Include(w => w.SiteAddress)
                                                    .Include(w => w.Organisation.BusinessAddress)
                                                    .ToListAsync();
                
                if (sentOnList != null && sentOnList.Count > 0)
                {
                    foreach (var sentOn in sentOnList)
                    {
                        var returnSearchedAatfAddressListData = new WeeeSearchedAnAatfListData()
                        {
                            ApprovalNumber = sentOn.ApprovalNumber,
                            WeeeAatfId = sentOn.Id,
                            OperatorAddress = new AatfAddressData(sentOn.Organisation.Name, sentOn.Organisation.BusinessAddress.Address1, sentOn.Organisation.BusinessAddress.Address2, sentOn.Organisation.BusinessAddress.TownOrCity, sentOn.Organisation.BusinessAddress.CountyOrRegion, sentOn.Organisation.BusinessAddress.Postcode, sentOn.Organisation.BusinessAddress.CountryId, sentOn.Organisation.BusinessAddress.Country.Name),
                            SiteAddress = new AatfAddressData(sentOn.SiteAddress.Name, sentOn.SiteAddress.Address1, sentOn.SiteAddress.Address2, sentOn.SiteAddress.TownOrCity, sentOn.SiteAddress.CountyOrRegion, sentOn.SiteAddress.Postcode, sentOn.SiteAddress.CountryId, sentOn.SiteAddress.Country.Name)
                        };
                        returnSearchedAatfAddressListDatas.Add(returnSearchedAatfAddressListData);
                    }
                }
                return returnSearchedAatfAddressListDatas;
            }
            else
            {
                var selectedAatf = await context.Aatfs.Where(a => a.Id == currentSelectedAatfId).SingleOrDefaultAsync();

                var result = await context.Aatfs.Where(x => (x.Name.Contains(searchedTerm) ||
                                                       x.ApprovalNumber.Contains(searchedTerm) ||
                                                       x.Organisation.Name.Contains(searchedTerm)) &&
                                                      (x.Id != currentSelectedAatfId && x.ComplianceYear == selectedAatf.ComplianceYear))
                                         .Select(x => new ReturnAatfAddressResult { SearchTermId = x.Id, SearchTermName = x.Name })
                                         .OrderBy(x => x.SearchTermName)
                                         .ToListAsync();

                return returnSearchedAatfAddressListDatas;
            }
        }
    }
}
