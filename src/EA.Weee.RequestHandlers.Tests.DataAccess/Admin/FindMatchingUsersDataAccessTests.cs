namespace EA.Weee.RequestHandlers.Tests.DataAccess.Admin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.Helpers;
    using Domain;
    using RequestHandlers.Admin;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class FindMatchingUsersDataAccessTests
    {
        public FindMatchingUsersDataAccessTests()
        {
        }

        [Fact]
        public async void GetOrganisationUsers_SameUserRejectedMultipleTimesForOneOrganisation_OnlyReturnsOneRejection()
        {
            using (var dbWrapper = new DatabaseWrapper())
            {
                // Add AspNet user user
                var modelHelper = new ModelHelper(dbWrapper.Model);               
                var aspNetUser = modelHelper.CreateUser("My username", IdType.Guid);
                dbWrapper.Model.SaveChanges();

                // Add organisation
                var uniqueOrgName = "Test Org " + Guid.NewGuid();
                var country = dbWrapper.WeeeContext.Countries.First();
                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(uniqueOrgName);
                organisation.AddOrUpdateMainContactPerson(new Domain.Organisation.Contact("First name", "Last name", "Developer"));
                organisation.AddOrUpdateAddress(AddressType.OrganisationAddress, new Domain.Organisation.Address(
                    "Address line 1", 
                    string.Empty, 
                    "Town", 
                    string.Empty, 
                    string.Empty,
                    country, 
                    "01234567890",
                    "a@b.c"));
                organisation.CompleteRegistration();
                dbWrapper.WeeeContext.Organisations.Add(organisation);
                dbWrapper.WeeeContext.SaveChanges();

                // Add same organisation user multiple times, rejected
                var organisationUsers = new List<Domain.Organisation.OrganisationUser>
                {
                    new Domain.Organisation.OrganisationUser(Guid.Parse(aspNetUser.Id),
                        organisation.Id,
                        UserStatus.Rejected),
                    new Domain.Organisation.OrganisationUser(Guid.Parse(aspNetUser.Id),
                        organisation.Id,
                        UserStatus.Rejected),
                    new Domain.Organisation.OrganisationUser(Guid.Parse(aspNetUser.Id),
                        organisation.Id,
                        UserStatus.Rejected)
                };

                dbWrapper.WeeeContext.OrganisationUsers.AddRange(organisationUsers);
                dbWrapper.WeeeContext.SaveChanges();

                var dataAccess = new FindMatchingUsersDataAccess(dbWrapper.WeeeContext);

                var result = (await dataAccess.GetOrganisationUsers())
                    .Where(ou => ou.OrganisationName == uniqueOrgName);

                Assert.Single(result);
            }
        }

        [Theory]
        [InlineData(Core.Shared.UserStatus.Active)]
        [InlineData(Core.Shared.UserStatus.Inactive)]
        [InlineData(Core.Shared.UserStatus.Pending)]
        public async void GetOrganisationUsers_SameUserRejectedMultipleTimesForOneOrganisation_ButAlsoHasAnotherStatus_ReturnsTheOtherStatus(Core.Shared.UserStatus status)
        {
            using (var dbWrapper = new DatabaseWrapper())
            {
                // Add AspNet user user
                var modelHelper = new ModelHelper(dbWrapper.Model);
                var aspNetUser = modelHelper.CreateUser("My username", IdType.Guid);
                dbWrapper.Model.SaveChanges();

                // Add organisation
                var uniqueOrgName = "Test Org " + Guid.NewGuid();
                var country = dbWrapper.WeeeContext.Countries.First();
                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(uniqueOrgName);
                organisation.AddOrUpdateMainContactPerson(new Domain.Organisation.Contact("First name", "Last name", "Developer"));
                organisation.AddOrUpdateAddress(AddressType.OrganisationAddress, new Domain.Organisation.Address(
                    "Address line 1",
                    string.Empty,
                    "Town",
                    string.Empty,
                    string.Empty,
                    country,
                    "01234567890",
                    "a@b.c"));
                organisation.CompleteRegistration();
                dbWrapper.WeeeContext.Organisations.Add(organisation);
                dbWrapper.WeeeContext.SaveChanges();

                // Add same organisation user multiple times, rejected
                var rejectedOrganisationUsers = new List<Domain.Organisation.OrganisationUser>
                {
                    new Domain.Organisation.OrganisationUser(Guid.Parse(aspNetUser.Id),
                        organisation.Id,
                        UserStatus.Rejected),
                    new Domain.Organisation.OrganisationUser(Guid.Parse(aspNetUser.Id),
                        organisation.Id,
                        UserStatus.Rejected),
                    new Domain.Organisation.OrganisationUser(Guid.Parse(aspNetUser.Id),
                        organisation.Id,
                        UserStatus.Rejected)
                };

                var otherOrganisationUser = new Domain.Organisation.OrganisationUser(Guid.Parse(aspNetUser.Id),
                    organisation.Id,
                    status.ToDomainEnumeration<UserStatus>());

                dbWrapper.WeeeContext.OrganisationUsers.AddRange(rejectedOrganisationUsers);
                dbWrapper.WeeeContext.OrganisationUsers.Add(otherOrganisationUser);
                dbWrapper.WeeeContext.SaveChanges();

                var dataAccess = new FindMatchingUsersDataAccess(dbWrapper.WeeeContext);

                var result = (await dataAccess.GetOrganisationUsers())
                    .Where(ou => ou.OrganisationName == uniqueOrgName);

                Assert.Single(result);

                var organisationUser = result.Single();
                Assert.Equal(status, organisationUser.Status);
            }
        }
    }
}
