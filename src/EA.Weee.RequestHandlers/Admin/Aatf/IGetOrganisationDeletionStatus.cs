namespace EA.Weee.RequestHandlers.Admin.Aatf
{
    using System;
    using System.Threading.Tasks;
    using Core.Admin;

    public interface IGetOrganisationDeletionStatus
    {
        Task<CanOrganisationBeDeletedFlags> Validate(Guid organisationId, int year);
    }
}