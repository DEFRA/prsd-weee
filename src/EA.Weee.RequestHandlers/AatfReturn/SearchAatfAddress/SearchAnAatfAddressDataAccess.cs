namespace EA.Weee.RequestHandlers.AatfReturn.SearchAatfAddress
{
    using EA.Prsd.Core;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.DataAccess;
    using EA.Weee.RequestHandlers.AatfReturn.ObligatedSentOn;
    using EA.Weee.Requests.AatfReturn;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    public class SearchAnAatfAddressDataAccess : ISearchAnAatfAddressDataAccess
    {
        private readonly WeeeContext context;
        private readonly IWeeeSentOnDataAccess getSentOnAatfSiteDataAccess;

        public SearchAnAatfAddressDataAccess(WeeeContext context, IWeeeSentOnDataAccess getSentOnAatfSiteDataAccess)
        {
            Guard.ArgumentNotNull(() => context, context);

            this.context = context;
            this.getSentOnAatfSiteDataAccess = getSentOnAatfSiteDataAccess;
        }

        public async Task<List<ReturnAatfAddressResult>> GetSearchAnAatfAddressBySearchTerm(GetSearchAatfAddress searchAatfAddress)
        {
            var selectedAatf = await context.Aatfs.Where(a => a.Id == searchAatfAddress.CurrentSelectedAatfId).SingleOrDefaultAsync();

            var weeeSentOnList = new List<WeeeSentOnData>();
            var weeeSentOn = await getSentOnAatfSiteDataAccess.GetWeeeSentOnByReturnAndAatf(searchAatfAddress.CurrentSelectedAatfId, searchAatfAddress.CurrentSelectedReturnId);

            var aatfIdList = new List<Guid>();
            aatfIdList.Add(searchAatfAddress.CurrentSelectedAatfId);

            foreach (var item in weeeSentOn)
            {
                var aatfData = context.Aatfs.Where(x => x.SiteAddressId == item.SiteAddressId && x.Organisation.BusinessAddressId == item.OperatorAddressId).SingleOrDefault();
                if (aatfData != null)
                {
                    aatfIdList.Add(aatfData.Id);
                }
            }

            if (searchAatfAddress.IsGeneralSearch)
            {
                var resultlist = await context.Aatfs
                                         .Where(x => (x.Name.Contains(searchAatfAddress.SearchTerm) ||
                                                      x.ApprovalNumber.Contains(searchAatfAddress.SearchTerm) ||
                                                      x.Organisation.Name.Contains(searchAatfAddress.SearchTerm)) &&
                                                     (!(aatfIdList.Contains(x.Id)) && x.ComplianceYear == selectedAatf.ComplianceYear))
                                         .Select(x => new ReturnAatfAddressResult { SearchTermId = x.Id, SearchTermName = x.Name, OrganisationId = x.Organisation.Id })
                                         .OrderBy(x => x.SearchTermName)
                                         .ToListAsync();

                return resultlist;
            }
            else
            {
                var resultlist = await context.Aatfs.Where(x => (x.Name.Contains(searchAatfAddress.SearchTerm) ||
                                                       x.ApprovalNumber.Contains(searchAatfAddress.SearchTerm) ||
                                                       x.Organisation.Name.Contains(searchAatfAddress.SearchTerm)) &&
                                                      (!(aatfIdList.Contains(x.Id)) && x.ComplianceYear == selectedAatf.ComplianceYear))

                                         .Select(x => new ReturnAatfAddressResult { SearchTermId = x.Id, SearchTermName = (x.Name + " - " + x.ApprovalNumber), OrganisationId = x.Organisation.Id })
                                         .Take(25)
                                         .OrderBy(x => x.SearchTermName)
                                         .ToListAsync();

                return resultlist;
            }
        }
    }
}
