namespace EA.Weee.RequestHandlers.Admin.DeleteAatf.DeleteValidation
{
    using System;
    using System.Threading.Tasks;
    using Core.Admin;

    public interface IGetOrganisationDeletionStatus
    {
        Task<CanOrganisationBeDeletedFlags> Validate(Guid organisationId, int year);
    }
}