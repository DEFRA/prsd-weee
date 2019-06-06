namespace EA.Weee.RequestHandlers.Factories
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.AatfReturn;
    using EA.Weee.Domain.DataReturns;
    using FacilityType = Core.AatfReturn.FacilityType;

    public class ReturnFactoryDataAccess : IReturnFactoryDataAccess
    {
        private readonly WeeeContext context;

        public ReturnFactoryDataAccess(WeeeContext context)
        {
            this.context = context;
        }

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

        public async Task<bool> ValidateReturnQuarter(Guid organisationId, int year, QuarterType quarterType, EA.Weee.Core.AatfReturn.FacilityType facilityType)
        {
            return await context.Returns
                .AnyAsync(r => r.Organisation.Id == organisationId
                               //&& r.FacilityType.Value == (int)facilityType
                               && r.Quarter.Year == year
                               && (int)r.Quarter.Q == (int)quarterType);
        }
    }
}
