namespace EA.Weee.RequestHandlers.Admin.Aatf
{
    using Core.Admin;
    using System;
    using System.Threading.Tasks;

    public interface IGetAatfDeletionStatus
    {
        Task<CanAatfBeDeletedFlags> Validate(Guid aatfId);
    }
}