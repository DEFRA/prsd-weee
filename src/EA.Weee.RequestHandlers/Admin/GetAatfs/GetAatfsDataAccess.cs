namespace EA.Weee.RequestHandlers.Admin.GetAatfs
{
    using DataAccess;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain.AatfReturn;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    public class GetAatfsDataAccess : IGetAatfsDataAccess
    {
        private readonly WeeeContext context;
        public GetAatfsDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<Aatf> GetAatfById(Guid id)
        {
            return await context.Aatfs.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Aatf>> GetAatfs()
        {
            return await context.Aatfs.ToListAsync();
        }

        public async Task<List<Aatf>> GetFilteredAatfs(AatfFilter filter)
        {
            var aatfList = await context.Aatfs.GroupBy(a => a.AatfId)
                .Select(x => x.OrderByDescending(a => a.ComplianceYear).FirstOrDefault())
                .Where(a =>
                (filter.Name == null || filter.Name.Trim() == string.Empty ||
                    a.Name.ToLower().Contains(filter.Name.ToLower())) &&
                (filter.ApprovalNumber == null || filter.ApprovalNumber.Trim() == string.Empty ||
                    a.ApprovalNumber.ToLower().Contains(filter.ApprovalNumber.ToLower())))
                .ToListAsync();

            if (filter.SelectedAuthority != null && filter.SelectedAuthority.Count > 0)
            {
                aatfList = aatfList.Where(x => filter.SelectedAuthority.Contains(x.CompetentAuthority.Id)).ToList();
            }
            if (filter.SelectedStatus != null && filter.SelectedStatus.Count > 0)
            {
                aatfList = aatfList.Where(x => filter.SelectedStatus.Contains(x.AatfStatus.Value)).ToList();
            }

            return aatfList;
        }

        public async Task<List<Aatf>> GetLatestAatfs()
        {
            return await context.Aatfs
                .GroupBy(a => a.AatfId)
                .Select(x => x.OrderByDescending(a => a.ComplianceYear).FirstOrDefault())
                .ToListAsync();
        }

        public async Task<List<Aatf>> GetAatfsBySiteAddressId(Guid siteAddressId)
        {
            return await context.Aatfs
                                .Where(a => a.SiteAddressId == siteAddressId)
                                .ToListAsync();
        }
    }
}