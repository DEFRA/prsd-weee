namespace EA.Weee.RequestHandlers.Admin.Aatf
{
    using System;
    using System.Threading.Tasks;
    using Core.Admin;
    using Domain.AatfReturn;

    public class GetAatfApprovalDateChangeStatus
    {
        public GetAatfApprovalDateChangeStatus()
        {
        }

        public async Task<CanApprovalDateBeChangedFlags> Validate(Aatf aatf, DateTime newApprovalDate)
        {
            var result = new CanApprovalDateBeChangedFlags();

            // change GetAnnualQuarter(Quarter quarter) or use to create list of all quarters, need this to determine if moved a quarter
            if (aatf.ApprovalDate.Equals(newApprovalDate))
            {
                return result;
            }

            return result;
        }
    }
}
