namespace EA.Weee.RequestHandlers.Organisations.JoinOrganisation.DataAccess
{
    using Domain.Organisation;
    using System;
    using System.Threading.Tasks;

    public interface IJoinOrganisationDataAccess
    {
        Task<JoinOrganisationResult> JoinOrganisation(OrganisationUser organisationUser);

        Task<bool> DoesUserExist(Guid userId);

        Task<bool> DoesOrganisationExist(Guid organisationId);
    }
}
