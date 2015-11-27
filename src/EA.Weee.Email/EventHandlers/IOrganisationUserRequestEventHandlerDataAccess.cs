namespace EA.Weee.Email.EventHandlers
{
    using Domain.Organisation;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IOrganisationUserRequestEventHandlerDataAccess
    {
        /// <summary>
        /// Fetches a distinct list of all users who are actively associated with the specified organisation
        /// ordered by their email address.
        /// </summary>
        /// <param name="organisationId"></param>
        /// <returns></returns>
        Task<IEnumerable<OrganisationUser>> FetchActiveOrganisationUsers(Guid organisationId);
    }
}
