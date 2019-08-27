namespace EA.Weee.RequestHandlers.Admin.Aatf
{
    using Core.Admin;
    using Domain.AatfReturn;
    using System;
    using System.Threading.Tasks;

    public interface IGetAatfApprovalDateChangeStatus
    {
        Task<CanApprovalDateBeChangedFlags> Validate(Aatf aatf, DateTime newApprovalDate);
    }
}