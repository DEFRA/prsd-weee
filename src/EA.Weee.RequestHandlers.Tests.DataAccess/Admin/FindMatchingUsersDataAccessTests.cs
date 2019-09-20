namespace EA.Weee.RequestHandlers.Tests.DataAccess.Admin
{
    using AutoFixture;
    using Core.Helpers;
    using Domain.User;
    using EA.Weee.Core.Admin;
    using EA.Weee.Core.User;
    using RequestHandlers.Admin;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Weee.Tests.Core.Model;
    using Xunit;
    using Organisation = Domain.Organisation.Organisation;

    public class FindMatchingUsersDataAccessTests
    {
        private readonly Fixture fixture;

        public FindMatchingUsersDataAccessTests()
        {
            fixture = new Fixture();
        }

        [Fact]
        public async void GetOrganisationUsers_SameUserRejectedMultipleTimesForOneOrganisation_OnlyReturnsOneRejection()
        {
            using (var dbWrapper = new DatabaseWrapper())
            {
                // Add AspNet user
                var modelHelper = new ModelHelper(dbWrapper.Model);
                var aspNetUser = modelHelper.CreateUser("My username", IdType.Guid);
                dbWrapper.Model.SaveChanges();

                // Add organisation
                var uniqueOrgName = "Test Org " + Guid.NewGuid();
                var organisation = Organisation.CreateSoleTrader(uniqueOrgName);
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

                var result = (await dataAccess.GetOrganisationUsers(new UserFilter()))
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
                // Add AspNet user
                var modelHelper = new ModelHelper(dbWrapper.Model);
                var aspNetUser = modelHelper.CreateUser("My username", IdType.Guid);
                dbWrapper.Model.SaveChanges();

                // Add organisation
                var uniqueOrgName = "Test Org " + Guid.NewGuid();
                var organisation = Organisation.CreateSoleTrader(uniqueOrgName);
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

                var result = (await dataAccess.GetOrganisationUsers(new UserFilter()))
                    .Where(ou => ou.OrganisationName == uniqueOrgName);

                Assert.Single(result);

                var organisationUser = result.Single();
                Assert.Equal(status, organisationUser.Status);
            }
        }

        [Fact]
        public async void GetOrganisationUsers_WithFirstNameFilter_ReturnsFilteredResults()
        {
            using (var dbWrapper = new DatabaseWrapper())
            {
                // Add AspNet users
                var modelHelper = new ModelHelper(dbWrapper.Model);
                var userOne = modelHelper.CreateUser(fixture.Create<string>(), IdType.Guid, "Marshall", "Eriksen");
                var userTwo = modelHelper.CreateUser(fixture.Create<string>(), IdType.Guid, "Lily", "Aldrin");
                var userThree = modelHelper.CreateUser(fixture.Create<string>(), IdType.Guid, "Barney", "Stinson");
                dbWrapper.Model.SaveChanges();

                // Add organisation
                var organisationName = fixture.Create<string>();
                var organisation = Organisation.CreateSoleTrader(organisationName);
                organisation.CompleteRegistration();
                dbWrapper.WeeeContext.Organisations.Add(organisation);
                dbWrapper.WeeeContext.SaveChanges();

                // Add organisation users
                var organisationUsers = new List<Domain.Organisation.OrganisationUser>
                {
                    new Domain.Organisation.OrganisationUser(Guid.Parse(userOne.Id), organisation.Id, UserStatus.Active),
                    new Domain.Organisation.OrganisationUser(Guid.Parse(userTwo.Id), organisation.Id, UserStatus.Active),
                    new Domain.Organisation.OrganisationUser(Guid.Parse(userThree.Id), organisation.Id, UserStatus.Active)
                };

                dbWrapper.WeeeContext.OrganisationUsers.AddRange(organisationUsers);
                dbWrapper.WeeeContext.SaveChanges();

                var dataAccess = new FindMatchingUsersDataAccess(dbWrapper.WeeeContext);

                var filter = new UserFilter { Name = "il" };

                var result = await dataAccess.GetOrganisationUsers(filter);

                Assert.Single(result);

                var organisationUser = result.Single();
                Assert.Equal(userTwo.FirstName, organisationUser.FirstName);
                Assert.Equal(userTwo.Surname, organisationUser.LastName);
            }
        }

        [Fact]
        public async void GetOrganisationUsers_WithSurnameFilter_ReturnsFilteredResults()
        {
            using (var dbWrapper = new DatabaseWrapper())
            {
                // Add AspNet users
                var modelHelper = new ModelHelper(dbWrapper.Model);
                var userOne = modelHelper.CreateUser(fixture.Create<string>(), IdType.Guid, "Marshall", "Eriksen");
                var userTwo = modelHelper.CreateUser(fixture.Create<string>(), IdType.Guid, "Lily", "Aldrin");
                var userThree = modelHelper.CreateUser(fixture.Create<string>(), IdType.Guid, "Barney", "Stinson");
                dbWrapper.Model.SaveChanges();

                // Add organisation
                var organisationName = fixture.Create<string>();
                var organisation = Organisation.CreateSoleTrader(organisationName);
                organisation.CompleteRegistration();
                dbWrapper.WeeeContext.Organisations.Add(organisation);
                dbWrapper.WeeeContext.SaveChanges();

                // Add organisation users
                var organisationUsers = new List<Domain.Organisation.OrganisationUser>
                {
                    new Domain.Organisation.OrganisationUser(Guid.Parse(userOne.Id), organisation.Id, UserStatus.Active),
                    new Domain.Organisation.OrganisationUser(Guid.Parse(userTwo.Id), organisation.Id, UserStatus.Active),
                    new Domain.Organisation.OrganisationUser(Guid.Parse(userThree.Id), organisation.Id, UserStatus.Active)
                };

                dbWrapper.WeeeContext.OrganisationUsers.AddRange(organisationUsers);
                dbWrapper.WeeeContext.SaveChanges();

                var dataAccess = new FindMatchingUsersDataAccess(dbWrapper.WeeeContext);

                var filter = new UserFilter { Name = "Ik" };

                var result = await dataAccess.GetOrganisationUsers(filter);

                Assert.Single(result);

                var organisationUser = result.Single();
                Assert.Equal(userOne.FirstName, organisationUser.FirstName);
                Assert.Equal(userOne.Surname, organisationUser.LastName);
            }
        }

        [Fact]
        public async void GetOrganisationUsers_WithFullNameFilter_ReturnsFilteredResults()
        {
            using (var dbWrapper = new DatabaseWrapper())
            {
                // Add AspNet users
                var modelHelper = new ModelHelper(dbWrapper.Model);
                var userOne = modelHelper.CreateUser(fixture.Create<string>(), IdType.Guid, "Marshall", "Eriksen");
                var userTwo = modelHelper.CreateUser(fixture.Create<string>(), IdType.Guid, "Lily", "Aldrin");
                var userThree = modelHelper.CreateUser(fixture.Create<string>(), IdType.Guid, "Barney", "Stinson");
                dbWrapper.Model.SaveChanges();

                // Add organisation
                var organisationName = fixture.Create<string>();
                var organisation = Organisation.CreateSoleTrader(organisationName);
                organisation.CompleteRegistration();
                dbWrapper.WeeeContext.Organisations.Add(organisation);
                dbWrapper.WeeeContext.SaveChanges();

                // Add organisation users
                var organisationUsers = new List<Domain.Organisation.OrganisationUser>
                {
                    new Domain.Organisation.OrganisationUser(Guid.Parse(userOne.Id), organisation.Id, UserStatus.Active),
                    new Domain.Organisation.OrganisationUser(Guid.Parse(userTwo.Id), organisation.Id, UserStatus.Active),
                    new Domain.Organisation.OrganisationUser(Guid.Parse(userThree.Id), organisation.Id, UserStatus.Active)
                };

                dbWrapper.WeeeContext.OrganisationUsers.AddRange(organisationUsers);
                dbWrapper.WeeeContext.SaveChanges();

                var dataAccess = new FindMatchingUsersDataAccess(dbWrapper.WeeeContext);

                var filter = new UserFilter { Name = "y s" };

                var result = await dataAccess.GetOrganisationUsers(filter);

                Assert.Single(result);

                var organisationUser = result.Single();
                Assert.Equal(userThree.FirstName, organisationUser.FirstName);
                Assert.Equal(userThree.Surname, organisationUser.LastName);
            }
        }

        [Fact]
        public async void GetOrganisationUsers_WithFullNameFilter_ReturnsMultipleFilteredResults()
        {
            using (var dbWrapper = new DatabaseWrapper())
            {
                // Add AspNet users
                var modelHelper = new ModelHelper(dbWrapper.Model);
                var userOne = modelHelper.CreateUser(fixture.Create<string>(), IdType.Guid, "Marshall", "Eriksen");
                var userTwo = modelHelper.CreateUser(fixture.Create<string>(), IdType.Guid, "Lily", "Aldrin");
                var userThree = modelHelper.CreateUser(fixture.Create<string>(), IdType.Guid, "Barney", "Stinson");
                dbWrapper.Model.SaveChanges();

                // Add organisation
                var organisationName = fixture.Create<string>();
                var organisation = Organisation.CreateSoleTrader(organisationName);
                organisation.CompleteRegistration();
                dbWrapper.WeeeContext.Organisations.Add(organisation);
                dbWrapper.WeeeContext.SaveChanges();

                // Add organisation users
                var organisationUsers = new List<Domain.Organisation.OrganisationUser>
                {
                    new Domain.Organisation.OrganisationUser(Guid.Parse(userOne.Id), organisation.Id, UserStatus.Active),
                    new Domain.Organisation.OrganisationUser(Guid.Parse(userTwo.Id), organisation.Id, UserStatus.Active),
                    new Domain.Organisation.OrganisationUser(Guid.Parse(userThree.Id), organisation.Id, UserStatus.Active)
                };

                dbWrapper.WeeeContext.OrganisationUsers.AddRange(organisationUsers);
                dbWrapper.WeeeContext.SaveChanges();

                var dataAccess = new FindMatchingUsersDataAccess(dbWrapper.WeeeContext);

                var filter = new UserFilter { Name = "ar" };

                var result = await dataAccess.GetOrganisationUsers(filter);

                Assert.Equal(2, result.Count());

                Assert.Single(result.Where(r => r.FirstName == userOne.FirstName && r.LastName == userOne.Surname));
                Assert.Single(result.Where(r => r.FirstName == userThree.FirstName && r.LastName == userThree.Surname));
            }
        }

        [Fact]
        public async void GetOrganisationUsers_WithOrganisationNameFilter_ReturnsFilteredResults()
        {
            using (var dbWrapper = new DatabaseWrapper())
            {
                // Add AspNet user
                var modelHelper = new ModelHelper(dbWrapper.Model);
                var user = modelHelper.CreateUser(fixture.Create<string>(), IdType.Guid, fixture.Create<string>().Substring(0, 10), fixture.Create<string>().Substring(0, 10));
                dbWrapper.Model.SaveChanges();

                // Add organisations
                var organisationOneName = "Waste";
                var organisationTwoName = "Trash";
                var organisationThreeName = "Recycling";
                var organisationOne = Organisation.CreateSoleTrader(organisationOneName);
                var organisationTwo = Organisation.CreatePartnership(organisationTwoName);
                var organisationThree = Organisation.CreateRegisteredCompany(organisationThreeName, fixture.Create<string>().Substring(0, 15));
                organisationOne.CompleteRegistration();
                organisationTwo.CompleteRegistration();
                organisationThree.CompleteRegistration();
                dbWrapper.WeeeContext.Organisations.Add(organisationOne);
                dbWrapper.WeeeContext.Organisations.Add(organisationTwo);
                dbWrapper.WeeeContext.Organisations.Add(organisationThree);
                dbWrapper.WeeeContext.SaveChanges();

                // Add organisation users
                var organisationUsers = new List<Domain.Organisation.OrganisationUser>
                {
                    new Domain.Organisation.OrganisationUser(Guid.Parse(user.Id), organisationOne.Id, UserStatus.Active),
                    new Domain.Organisation.OrganisationUser(Guid.Parse(user.Id), organisationTwo.Id, UserStatus.Active),
                    new Domain.Organisation.OrganisationUser(Guid.Parse(user.Id), organisationThree.Id, UserStatus.Active)
                };

                dbWrapper.WeeeContext.OrganisationUsers.AddRange(organisationUsers);
                dbWrapper.WeeeContext.SaveChanges();

                var dataAccess = new FindMatchingUsersDataAccess(dbWrapper.WeeeContext);

                var filter = new UserFilter { OrganisationName = "recycl" };

                var result = await dataAccess.GetOrganisationUsers(filter);

                Assert.Single(result);

                var organisationUser = result.Single();
                Assert.Equal(organisationThreeName, organisationUser.OrganisationName);
            }
        }

        [Fact]
        public async void GetOrganisationUsers_WithOrganisationNameFilter_ReturnsMultipleFilteredResults()
        {
            using (var dbWrapper = new DatabaseWrapper())
            {
                // Add AspNet user
                var modelHelper = new ModelHelper(dbWrapper.Model);
                var user = modelHelper.CreateUser(fixture.Create<string>(), IdType.Guid, fixture.Create<string>().Substring(0, 10), fixture.Create<string>().Substring(0, 10));
                dbWrapper.Model.SaveChanges();

                // Add organisations
                var organisationOneName = "Waste";
                var organisationTwoName = "Trash";
                var organisationThreeName = "Recycling";
                var organisationOne = Organisation.CreateSoleTrader(organisationOneName);
                var organisationTwo = Organisation.CreatePartnership(organisationTwoName);
                var organisationThree = Organisation.CreateRegisteredCompany(organisationThreeName, fixture.Create<string>().Substring(0, 15));
                organisationOne.CompleteRegistration();
                organisationTwo.CompleteRegistration();
                organisationThree.CompleteRegistration();
                dbWrapper.WeeeContext.Organisations.Add(organisationOne);
                dbWrapper.WeeeContext.Organisations.Add(organisationTwo);
                dbWrapper.WeeeContext.Organisations.Add(organisationThree);
                dbWrapper.WeeeContext.SaveChanges();

                // Add organisation users
                var organisationUsers = new List<Domain.Organisation.OrganisationUser>
                {
                    new Domain.Organisation.OrganisationUser(Guid.Parse(user.Id), organisationOne.Id, UserStatus.Active),
                    new Domain.Organisation.OrganisationUser(Guid.Parse(user.Id), organisationTwo.Id, UserStatus.Active),
                    new Domain.Organisation.OrganisationUser(Guid.Parse(user.Id), organisationThree.Id, UserStatus.Active)
                };

                dbWrapper.WeeeContext.OrganisationUsers.AddRange(organisationUsers);
                dbWrapper.WeeeContext.SaveChanges();

                var dataAccess = new FindMatchingUsersDataAccess(dbWrapper.WeeeContext);

                var filter = new UserFilter { OrganisationName = "as" };

                var result = await dataAccess.GetOrganisationUsers(filter);

                Assert.Equal(2, result.Count());

                Assert.Single(result.Where(r => r.OrganisationName == organisationOneName));
                Assert.Single(result.Where(r => r.OrganisationName == organisationTwoName));
            }
        }

        [Fact]
        public async void GetOrganisationUsers_WithOrganisationNameFilterAndMatchingTradingName_DoesNotReturnWhenOrganisationNameDoesNotMatch()
        {
            using (var dbWrapper = new DatabaseWrapper())
            {
                // Add AspNet user
                var modelHelper = new ModelHelper(dbWrapper.Model);
                var user = modelHelper.CreateUser(fixture.Create<string>(), IdType.Guid, fixture.Create<string>().Substring(0, 10), fixture.Create<string>().Substring(0, 10));
                dbWrapper.Model.SaveChanges();

                // Add organisation
                var organisationName = "Waste";
                var organisationTradingName = "Trash";
                var organisation = Organisation.CreateSoleTrader(organisationName, organisationTradingName);
                organisation.CompleteRegistration();
                dbWrapper.WeeeContext.Organisations.Add(organisation);
                dbWrapper.WeeeContext.SaveChanges();

                // Add organisation users
                var organisationUsers = new List<Domain.Organisation.OrganisationUser>
                {
                    new Domain.Organisation.OrganisationUser(Guid.Parse(user.Id), organisation.Id, UserStatus.Active)
                };

                dbWrapper.WeeeContext.OrganisationUsers.AddRange(organisationUsers);
                dbWrapper.WeeeContext.SaveChanges();

                var dataAccess = new FindMatchingUsersDataAccess(dbWrapper.WeeeContext);

                var filter = new UserFilter { OrganisationName = "Trash" };

                var result = await dataAccess.GetOrganisationUsers(filter);

                Assert.Equal(0, result.Count());
            }
        }

        [Theory]
        [InlineData(Core.Shared.UserStatus.Active)]
        [InlineData(Core.Shared.UserStatus.Inactive)]
        [InlineData(Core.Shared.UserStatus.Pending)]
        [InlineData(Core.Shared.UserStatus.Rejected)]
        public async void GetOrganisationUsers_WithStatusFilter_ReturnsFilteredResults(Core.Shared.UserStatus status)
        {
            using (var dbWrapper = new DatabaseWrapper())
            {
                // Add AspNet users
                var modelHelper = new ModelHelper(dbWrapper.Model);
                var activeUser = modelHelper.CreateUser(fixture.Create<string>(), IdType.Guid, fixture.Create<string>().Substring(0, 10), fixture.Create<string>().Substring(0, 10));
                var inactiveUser = modelHelper.CreateUser(fixture.Create<string>(), IdType.Guid, fixture.Create<string>().Substring(0, 10), fixture.Create<string>().Substring(0, 10));
                var pendingUser = modelHelper.CreateUser(fixture.Create<string>(), IdType.Guid, fixture.Create<string>().Substring(0, 10), fixture.Create<string>().Substring(0, 10));
                var rejectedUser = modelHelper.CreateUser(fixture.Create<string>(), IdType.Guid, fixture.Create<string>().Substring(0, 10), fixture.Create<string>().Substring(0, 10));
                dbWrapper.Model.SaveChanges();

                // Add organisation
                var organisationName = fixture.Create<string>();
                var organisation = Organisation.CreateSoleTrader(organisationName);
                organisation.CompleteRegistration();
                dbWrapper.WeeeContext.Organisations.Add(organisation);
                dbWrapper.WeeeContext.SaveChanges();

                // Add organisation users
                var organisationUsers = new List<Domain.Organisation.OrganisationUser>
                {
                    new Domain.Organisation.OrganisationUser(Guid.Parse(activeUser.Id), organisation.Id, UserStatus.Active),
                    new Domain.Organisation.OrganisationUser(Guid.Parse(inactiveUser.Id), organisation.Id, UserStatus.Inactive),
                    new Domain.Organisation.OrganisationUser(Guid.Parse(pendingUser.Id), organisation.Id, UserStatus.Pending),
                    new Domain.Organisation.OrganisationUser(Guid.Parse(rejectedUser.Id), organisation.Id, UserStatus.Rejected)
                };

                dbWrapper.WeeeContext.OrganisationUsers.AddRange(organisationUsers);
                dbWrapper.WeeeContext.SaveChanges();

                var dataAccess = new FindMatchingUsersDataAccess(dbWrapper.WeeeContext);

                var filter = new UserFilter { Status = status, OrganisationName = organisationName };

                var result = await dataAccess.GetOrganisationUsers(filter);

                UserSearchData organisationUser;
                if (status == Core.Shared.UserStatus.Active)
                {
                    Assert.Single(result.Where(r => r.FirstName == activeUser.FirstName));
                    organisationUser = result.Single(r => r.FirstName == activeUser.FirstName);
                }
                else
                {
                    Assert.Single(result);
                    organisationUser = result.Single();
                }

                Assert.Equal(status, organisationUser.Status);
            }
        }

        [Fact]
        public async void GetOrganisationUsers_WithStatusFilter_ReturnsMultipleFilteredResults()
        {
            using (var dbWrapper = new DatabaseWrapper())
            {
                // Add AspNet users
                var modelHelper = new ModelHelper(dbWrapper.Model);
                var activeUser = modelHelper.CreateUser(fixture.Create<string>(), IdType.Guid, fixture.Create<string>().Substring(0, 10), fixture.Create<string>().Substring(0, 10));
                var inactiveUser = modelHelper.CreateUser(fixture.Create<string>(), IdType.Guid, fixture.Create<string>().Substring(0, 10), fixture.Create<string>().Substring(0, 10));
                var pendingUser = modelHelper.CreateUser(fixture.Create<string>(), IdType.Guid, fixture.Create<string>().Substring(0, 10), fixture.Create<string>().Substring(0, 10));
                var rejectedThenPendingUser = modelHelper.CreateUser(fixture.Create<string>(), IdType.Guid, fixture.Create<string>().Substring(0, 10), fixture.Create<string>().Substring(0, 10));
                dbWrapper.Model.SaveChanges();

                // Add organisation
                var organisationName = fixture.Create<string>();
                var organisation = Organisation.CreateSoleTrader(organisationName);
                organisation.CompleteRegistration();
                dbWrapper.WeeeContext.Organisations.Add(organisation);
                dbWrapper.WeeeContext.SaveChanges();

                // Add organisation users
                var organisationUsers = new List<Domain.Organisation.OrganisationUser>
                {
                    new Domain.Organisation.OrganisationUser(Guid.Parse(activeUser.Id), organisation.Id, UserStatus.Active),
                    new Domain.Organisation.OrganisationUser(Guid.Parse(inactiveUser.Id), organisation.Id, UserStatus.Inactive),
                    new Domain.Organisation.OrganisationUser(Guid.Parse(pendingUser.Id), organisation.Id, UserStatus.Pending),
                    new Domain.Organisation.OrganisationUser(Guid.Parse(rejectedThenPendingUser.Id), organisation.Id, UserStatus.Rejected),
                    new Domain.Organisation.OrganisationUser(Guid.Parse(rejectedThenPendingUser.Id), organisation.Id, UserStatus.Pending)
                };

                dbWrapper.WeeeContext.OrganisationUsers.AddRange(organisationUsers);
                dbWrapper.WeeeContext.SaveChanges();

                var dataAccess = new FindMatchingUsersDataAccess(dbWrapper.WeeeContext);

                var filter = new UserFilter { Status = Core.Shared.UserStatus.Pending, OrganisationName = organisationName };

                var result = await dataAccess.GetOrganisationUsers(filter);

                Assert.Equal(2, result.Count());

                Assert.True(result.All(r => r.Status == Core.Shared.UserStatus.Pending));
            }
        }
    }
}
