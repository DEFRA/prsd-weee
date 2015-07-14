namespace EA.Weee.Requests.Tests.Unit.Helpers
{
    using System;
    using Domain.Organisation;
    using EA.Weee.Domain;

    internal class OrganisationUserHelper
    {
        internal OrganisationUser GetOrganisationUser(Guid userId, OrganisationUserStatus status)
        {   
            return new OrganisationUser(userId, new Guid(), status);
        }
    }
}
