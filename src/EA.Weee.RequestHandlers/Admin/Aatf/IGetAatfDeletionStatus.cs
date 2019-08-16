namespace EA.Weee.RequestHandlers.Admin.Aatf
{
    using System;
    using System.Threading.Tasks;
    using Core.Admin;

    public interface IGetAatfDeletionStatus
    {
        Task<CanAatfBeDeletedFlags> Validate(Guid aatfId);
    }
}