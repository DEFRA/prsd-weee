namespace EA.Weee.RequestHandlers.Admin.Aatf
{
    using Core.Admin;
    using DataAccess.DataAccess;
    using Domain.AatfReturn;
    using Factories;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using RequestHandlers.Aatf;

    public class GetAatfApprovalDateChangeStatus : IGetAatfApprovalDateChangeStatus
    {
        private readonly IAatfDataAccess aatfDataAccess;
        private readonly IQuarterWindowFactory quarterWindowFactory;
        private readonly IOrganisationDataAccess organisationDataAccess;

        public GetAatfApprovalDateChangeStatus(IAatfDataAccess aatfDataAccess,
            IQuarterWindowFactory quarterWindowFactory,
            IOrganisationDataAccess organisationDataAccess)
        {
            this.aatfDataAccess = aatfDataAccess;
            this.quarterWindowFactory = quarterWindowFactory;
            this.organisationDataAccess = organisationDataAccess;
        }

        public async Task<CanApprovalDateBeChangedFlags> Validate(Aatf aatf, DateTime newApprovalDate)
        {
            var result = new CanApprovalDateBeChangedFlags();

            if (aatf.ApprovalDate.HasValue)
            {
                var currentQuarter = await quarterWindowFactory.GetAnnualQuarterForDate(aatf.ApprovalDate.Value);
                var newQuarter = await quarterWindowFactory.GetAnnualQuarterForDate(newApprovalDate);

                if (aatf.ApprovalDate.Equals(newApprovalDate))
                {
                    return result;
                }

                if ((int)newQuarter > (int)currentQuarter)
                {
                    result |= CanApprovalDateBeChangedFlags.DateChanged;

                    var returns = await organisationDataAccess.GetReturnsByComplianceYear(aatf.Organisation.Id, aatf.ComplianceYear, aatf.FacilityType);

                    if (returns.Any(r => (int)r.Quarter.Q == (int)currentQuarter && r.ReturnStatus.Value == ReturnStatus.Created.Value))
                    {
                        result |= CanApprovalDateBeChangedFlags.HasStartedReturn;
                    }

                    if (returns.Any(r => (int)r.Quarter.Q == (int)currentQuarter && r.ReturnStatus.Value == ReturnStatus.Submitted.Value))
                    {
                        result |= CanApprovalDateBeChangedFlags.HasSubmittedReturn;
                    }

                    if (returns.Any(r => ((int)r.Quarter.Q == (int)currentQuarter && r.ReturnStatus.Value == ReturnStatus.Submitted.Value && r.ParentId != null)
                    || ((int)r.Quarter.Q == (int)currentQuarter && r.ReturnStatus.Value == ReturnStatus.Submitted.Value && r.ParentId != null)))
                    {
                        result |= CanApprovalDateBeChangedFlags.HasResubmission;
                    }

                    if (await aatfDataAccess.HasAatfOrganisationOtherAeOrAatf(aatf))
                    {
                        result |= CanApprovalDateBeChangedFlags.HasMultipleFacility;
                    }
                }
            }

            return result;
        }
    }
}
