namespace EA.Weee.RequestHandlers.AatfReturn.SearchAatfAddress
{
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.AatfReturn;
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

        public async Task<List<Aatf>> GetSearchAnAatfAddressBySearchTerm(string searchTerm)
        {
            return await context.Aatfs.Where(w => w.Name.Contains(searchTerm) ||
                                                  w.ApprovalNumber.Contains(searchTerm) ||
                                                  w.Organisation.Name.Contains(searchTerm))
                                      .GroupBy(a => a.AatfId)
                                      .Select(x => x.OrderByDescending(a => a.ComplianceYear).FirstOrDefault())
                                      .OrderBy(x => x.Name)
                                      .Take(5)
                                      .ToListAsync();
        }
    }
}
