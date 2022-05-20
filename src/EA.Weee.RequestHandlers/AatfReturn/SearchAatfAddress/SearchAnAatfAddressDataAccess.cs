namespace EA.Weee.RequestHandlers.AatfReturn.SearchAatfAddress
{
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.AatfReturn;
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
            this.context = context;
        }

        public async Task<List<Aatf>> GetSearchAnAatfAddressBySearchTerm(GetSearchAatfAddress searchAatfAddress)
        {
            var selectedAatf = await context.Aatfs.Where(a => a.Id == searchAatfAddress.CurrentSelectedAatfId).SingleOrDefaultAsync();

            return await context.Aatfs.Where(w => (w.Name.Contains(searchAatfAddress.SearchTerm) ||
                                                  w.ApprovalNumber.Contains(searchAatfAddress.SearchTerm) ||
                                                  w.Organisation.Name.Contains(searchAatfAddress.SearchTerm)) && 
                                                 (w.Id != searchAatfAddress.CurrentSelectedAatfId && 
                                                  w.ComplianceYear == selectedAatf.ComplianceYear))
                                      .GroupBy(a => a.AatfId)
                                      .Select(x => x.OrderByDescending(a => a.ComplianceYear).FirstOrDefault())
                                      .OrderBy(x => x.Name)
                                      .Take(5)
                                      .ToListAsync();
        }
    }
}
