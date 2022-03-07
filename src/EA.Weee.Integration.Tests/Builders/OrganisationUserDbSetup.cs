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
            instance = new OrganisationUser(Guid.Empty, Guid.Empty, UserStatus.Pending);

            return instance;
        }

        public OrganisationUserDbSetup WithUserIdAndOrganisationId(Guid userId, Guid organisationId)
        {
            instance.UpdateUserIdAndOrganisationId(userId, organisationId);

            return this;
        }

        public OrganisationUserDbSetup WithStatus(UserStatus status)
        {
            instance.UpdateUserStatus(status);

            return this;
        }
    }
}
