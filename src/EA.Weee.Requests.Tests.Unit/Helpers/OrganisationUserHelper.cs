namespace EA.Weee.Requests.Tests.Unit.Helpers
{
    using System;
    using EA.Weee.Domain;

    internal class OrganisationUserHelper
    {
        internal OrganisationUser GetApprovedOrganisationUser(Guid userId)
        {   
            return new OrganisationUser(userId, new Guid(), OrganisationUserStatus.Approved);
        }

        internal OrganisationUser GetPendingOrganisationUser(Guid userId)
        {
            return new OrganisationUser(userId, new Guid(), OrganisationUserStatus.Pending);
        }
    }
}
