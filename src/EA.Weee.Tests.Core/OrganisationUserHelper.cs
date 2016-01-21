namespace EA.Weee.Tests.Core
{
    using System;
    using Domain.Organisation;
    using Domain.User;

    public class OrganisationUserHelper
    {
        public OrganisationUser GetOrganisationUser(Guid userId, UserStatus status)
        {   
            return new OrganisationUser(userId, new Guid(), status);
        }
    }
}
