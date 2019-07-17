namespace EA.Weee.Email.EventHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain;
    using Domain.Organisation;
    using EA.Weee.Domain.User;

    public interface IOrganisationUserRequestEventHandlerDataAccess
    {
        /// <summary>
        /// Fetches a distinct list of all users who are actively associated with the specified organisation
        /// ordered by their email address.
        /// </summary>
        /// <param name="organisationId"></param>
        /// <returns></returns>
        Task<IEnumerable<OrganisationUser>> FetchActiveOrganisationUsers(Guid organisationId);

        Task<User> FetchUser(string userId);

        Task<Organisation> FetchOrganisation(Guid orgId);

        Task<IEnumerable<UKCompetentAuthority>> FetchCompetentAuthorities();
    }
}
