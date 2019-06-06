namespace EA.Weee.RequestHandlers.Factories
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.AatfReturn;
    using FacilityType = Core.AatfReturn.FacilityType;

    public class ReturnFactoryDataAccess : IReturnFactoryDataAccess
    {
        private readonly WeeeContext context;

        public ReturnFactoryDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<IList<Aatf>> FetchAatfsByOrganisationFacilityTypeListAndYear(Guid organisationId, int year, EA.Weee.Core.AatfReturn.FacilityType facilityType)
        {
            return await context.Aatfs
                .Where(a => a.Organisation.Id == organisationId 
                            && a.FacilityType.Value == (int)facilityType 
                            && a.ComplianceYear == year 
                            && a.ApprovalDate.HasValue).ToListAsync();
        }

        // TESTS
        public async Task<bool> ValidateAatfApprovalDate(Guid organisationId, DateTime date, EA.Weee.Core.AatfReturn.FacilityType facilityType)
        {
            return await context.Aatfs
                .AnyAsync(a => a.Organisation.Id == organisationId
                            && a.FacilityType.Value == (int)facilityType
                            && a.ComplianceYear == date.Year
                            && a.ApprovalDate.HasValue
                            && a.ApprovalDate < date.Date
                            && a.ApprovalDate != default(DateTime));
        }

        public async Task<bool> ValidateReturnQuarter(Guid organisationId, FacilityType facilityType)
        {
            return true;
        }
    }
}
