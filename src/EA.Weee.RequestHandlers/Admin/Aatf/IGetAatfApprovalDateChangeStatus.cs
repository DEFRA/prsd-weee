namespace EA.Weee.RequestHandlers.Admin.Aatf
{
    using System;
    using System.Threading.Tasks;
    using Core.Admin;
    using Domain.AatfReturn;

    public interface IGetAatfApprovalDateChangeStatus
    {
        Task<CanApprovalDateBeChangedFlags> Validate(Aatf aatf, DateTime newApprovalDate);
    }
}