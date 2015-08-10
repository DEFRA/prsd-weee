namespace EA.Weee.RequestHandlers.Tests.Unit.Helpers
{
    using System;
    using Domain;
    using Domain.Organisation;

    internal class OrganisationUserHelper
    {
        internal OrganisationUser GetOrganisationUser(Guid userId, UserStatus status)
        {   
            return new OrganisationUser(userId, new Guid(), status);
        }
    }
}
