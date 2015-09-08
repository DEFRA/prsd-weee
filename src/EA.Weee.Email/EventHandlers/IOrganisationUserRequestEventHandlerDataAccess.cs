namespace EA.Weee.Email.EventHandlers
{
    using EA.Weee.Domain;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    
    public interface IOrganisationUserRequestEventHandlerDataAccess
    {
        /// <summary>
        /// Fetches a distinct list of all users who are actively associated with the specified organisation
        /// ordered by their email address.
        /// </summary>
        /// <param name="organisationId"></param>
        /// <returns></returns>
        Task<IEnumerable<User>> FetchActiveOrganisationUsers(Guid organisationId);
    }
}
