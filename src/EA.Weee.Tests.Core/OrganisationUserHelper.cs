namespace EA.Weee.Tests.Core
{
    using Domain.Organisation;
    using Domain.User;
    using System;

    public class OrganisationUserHelper
    {
        public OrganisationUser GetOrganisationUser(Guid userId, UserStatus status)
        {
            return new OrganisationUser(userId, new Guid(), status);
        }
    }
}
