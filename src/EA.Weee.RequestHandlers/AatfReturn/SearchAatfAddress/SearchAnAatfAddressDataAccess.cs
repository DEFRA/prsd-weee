namespace EA.Weee.RequestHandlers.AatfReturn.SearchAatfAddress
{
    using EA.Prsd.Core;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.DataAccess;
    using EA.Weee.Requests.AatfReturn;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    public class SearchAnAatfAddressDataAccess : ISearchAnAatfAddressDataAccess
    {
        private readonly WeeeContext context;

        public SearchAnAatfAddressDataAccess(WeeeContext context)
        {
            Guard.ArgumentNotNull(() => context, context);

            this.context = context;
        }

        public async Task<List<ReturnAatfAddressResult>> GetSearchAnAatfAddressBySearchTerm(GetSearchAatfAddress searchAatfAddress)
        {
            var selectedAatf = await context.Aatfs.Where(a => a.Id == searchAatfAddress.CurrentSelectedAatfId).SingleOrDefaultAsync();

            if (searchAatfAddress.IsGeneralSearch)
            {
                var resultlist = await context.Aatfs
                                         .Where(x => (x.Name.Contains(searchAatfAddress.SearchTerm) ||
                                                       x.ApprovalNumber.Contains(searchAatfAddress.SearchTerm) ||
                                                       x.Organisation.Name.Contains(searchAatfAddress.SearchTerm)) &&
                                                      (x.Id != searchAatfAddress.CurrentSelectedAatfId && x.ComplianceYear == selectedAatf.ComplianceYear && x.AatfStatus.Value == AatfStatus.Approved.Value))
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
                                                      (x.Id != searchAatfAddress.CurrentSelectedAatfId && x.ComplianceYear == selectedAatf.ComplianceYear && x.AatfStatus.Value == AatfStatus.Approved.Value))

                                         .Select(x => new ReturnAatfAddressResult { SearchTermId = x.Id, SearchTermName = x.Name, OrganisationId = x.Organisation.Id })
                                         .Take(5)
                                         .OrderBy(x => x.SearchTermName)
                                         .ToListAsync();

                return resultlist;
            }
        }
    }
}
