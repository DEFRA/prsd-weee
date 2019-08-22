namespace EA.Weee.RequestHandlers.Factories
{
    using DataAccess;
    using EA.Weee.Domain.DataReturns;
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;

    public class ReturnFactoryDataAccess : IReturnFactoryDataAccess
    {
        private readonly WeeeContext context;

        public ReturnFactoryDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<bool> ValidateAatfApprovalDate(Guid organisationId, DateTime date, int year, EA.Weee.Core.AatfReturn.FacilityType facilityType)
        {
            return await context.Aatfs
                .AnyAsync(a => a.Organisation.Id == organisationId
                            && a.FacilityType.Value == (int)facilityType
                            && a.ComplianceYear == year
                            && a.ApprovalDate.HasValue
                            && a.ApprovalDate < date.Date
                            && a.ApprovalDate != default(DateTime));
        }

        public async Task<bool> HasReturnQuarter(Guid organisationId, int year, QuarterType quarterType, EA.Weee.Core.AatfReturn.FacilityType facilityType)
        {
            return await context.Returns
                .AnyAsync(r => r.Organisation.Id == organisationId
                               && r.FacilityType.Value == (int)facilityType
                               && r.Quarter.Year == year
                               && (int)r.Quarter.Q == (int)quarterType);
        }
    }
}
