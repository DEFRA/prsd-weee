namespace EA.Weee.RequestHandlers.Admin.RemoveAATFOrAeRecords
{
    using Domain.AatfReturn;
    using EA.Weee.Core.Admin;
    using EA.Weee.DataAccess;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    internal class GetAatfOrAeRetentionPeriodDataAccess : IGetAatfOrAeRetentionPeriodDataAccess
    {
        private readonly WeeeContext context;
        public GetAatfOrAeRetentionPeriodDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<List<Aatf>> GetFilteredAatfs(RemoveAatfOrAeFilter filter)
        {
            var userCompetentAuthority = context.CompetentAuthorityUsers.Where(x => x.UserId == filter.UserId).SingleOrDefault();
            var facilityTypeVal = (filter.FacilityType == Core.AatfReturn.FacilityType.Aatf) ? 1 : 2;

            var query = context.Aatfs.Where(x => x.CompetentAuthority.Id.Equals(userCompetentAuthority.CompetentAuthorityId) &&
                                                 x.FacilityType.Value.Equals(facilityTypeVal));

            if (!string.IsNullOrWhiteSpace(filter.Name))
            {
                query = query.Where(x => x.Name.ToLower().Contains(filter.Name.ToLower()));
            }

            if (!string.IsNullOrEmpty(filter.ApprovalNumber))
            {
                query = query.Where(x => x.ApprovalNumber.ToLower().Contains(filter.ApprovalNumber.ToLower()));
            }

            if (filter.ComplianceYear.HasValue)
            {
                // Specific year requested
                query = query.Where(x => x.ComplianceYear == filter.ComplianceYear.Value);
            }
            else
            {
                // No year supplied - show current year and previous 6 years
                var startYear = DateTime.UtcNow.Year - filter.RetenctionPeriod;

                query = query.Where(x => x.ComplianceYear <= startYear);
            }

            if (filter.SelectedStatus.HasValue)
            {
                query = query.Where(x => x.AatfStatus.Value.Equals(filter.SelectedStatus.Value));
            }

            var aatfList = await query.GroupBy(x => x.AatfId)
                                      .Select(x => x.OrderByDescending(a => a.ComplianceYear).FirstOrDefault())
                                      .OrderBy(x => x.ComplianceYear)
                                      .ThenBy(x => x.Name)
                                      .ToListAsync();

            return aatfList;
        }
    }
}
