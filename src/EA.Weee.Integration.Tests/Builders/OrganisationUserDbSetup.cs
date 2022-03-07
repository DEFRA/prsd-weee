namespace EA.Weee.Integration.Tests.Builders
{
    using System;
    using System.Linq;
    using Base;
    using Domain;
    using Domain.Organisation;
    using Domain.User;

    public class OrganisationUserDbSetup : DbTestDataBuilder<OrganisationUser, OrganisationUserDbSetup>
    {
        protected override OrganisationUser Instantiate()
        {
            Instance = new OrganisationUser(Guid.Empty, Guid.Empty, UserStatus.Pending);

            return Instance;
        }

        public OrganisationUserDbSetup WithUserIdAndOrganisationId(Guid userId, Guid organisationId)
        {
            Instance.UpdateUserIdAndOrganisationId(userId, organisationId);

            return this;
        }

        public OrganisationUserDbSetup WithStatus(UserStatus status)
        {
            Instance.UpdateUserStatus(status);

            return this;
        }
    }
}
