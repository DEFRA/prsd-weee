namespace EA.Weee.RequestHandlers.Admin.DeleteAatf.DeleteValidation
{
    using System;
    using System.Threading.Tasks;
    using Core.Admin;

    public interface IGetAatfDeletionStatus
    {
        Task<CanAatfBeDeletedFlags> Validate(Guid aatfId);
    }
}