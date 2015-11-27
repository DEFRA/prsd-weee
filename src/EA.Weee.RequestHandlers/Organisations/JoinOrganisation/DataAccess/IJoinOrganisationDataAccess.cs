namespace EA.Weee.RequestHandlers.Organisations.JoinOrganisation.DataAccess
{
    using System;
    using System.Threading.Tasks;
    using Domain.Organisation;

    public interface IJoinOrganisationDataAccess
    {
        Task<JoinOrganisationResult> JoinOrganisation(OrganisationUser organisationUser);

        Task<bool> DoesUserExist(Guid userId);

        Task<bool> DoesOrganisationExist(Guid organisationId);
    }
}
