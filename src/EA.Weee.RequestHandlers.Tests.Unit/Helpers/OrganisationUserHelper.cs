namespace EA.Weee.RequestHandlers.Tests.Unit.Helpers
{
    using System;
    using Domain.Organisation;

    internal class OrganisationUserHelper
    {
        internal OrganisationUser GetOrganisationUser(Guid userId, OrganisationUserStatus status)
        {   
            return new OrganisationUser(userId, new Guid(), status);
        }
    }
}
