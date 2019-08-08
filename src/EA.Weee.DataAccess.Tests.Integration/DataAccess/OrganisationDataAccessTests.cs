namespace EA.Weee.DataAccess.Tests.Integration.DataAccess
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using Domain.User;
    using FluentAssertions;
    using Weee.DataAccess.DataAccess;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class OrganisationDataAccessTests
    {
        private readonly Fixture fixture;

        public OrganisationDataAccessTests()
        {
            fixture = new Fixture();
        }

        [Fact]
        public async void DeleteOrganisation_GivenOrganisation_OrganisationShouldBeRemoved()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var organisationDataAccess = new OrganisationDataAccess(databaseWrapper.WeeeContext);

                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());
                var organisationDontRemove = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());

                databaseWrapper.WeeeContext.Organisations.Add(organisation);
                databaseWrapper.WeeeContext.Organisations.Add(organisationDontRemove);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                databaseWrapper.WeeeContext.Organisations.Where(o => o.Id == organisation.Id).Should().NotBeEmpty();

                await organisationDataAccess.Delete(organisation.Id);

                databaseWrapper.WeeeContext.Organisations.Where(o => o.Id == organisation.Id).Should().BeEmpty();
                databaseWrapper.WeeeContext.Organisations.Where(o => o.Id == organisationDontRemove.Id).Should().NotBeEmpty();
            }
        }

        [Fact]
        public async void DeleteOrganisation_GivenInvalidOrganisation_ArgumentExceptionExpected()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var organisationDataAccess = new OrganisationDataAccess(databaseWrapper.WeeeContext);

                var action = await Xunit.Record.ExceptionAsync(() => organisationDataAccess.Delete(Guid.Empty));

                action.Should().BeOfType<ArgumentException>();
            }
        }

        [Fact]
        public async void HasActiveUsers_GivenOrganisationWithNoUsers_FalseShouldBeReturned()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var organisationDataAccess = new OrganisationDataAccess(databaseWrapper.WeeeContext);

                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());

                databaseWrapper.WeeeContext.Organisations.Add(organisation);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                var result = await organisationDataAccess.HasActiveUsers(organisation.Id);

                result.Should().BeFalse();
            }
        }

        [Fact]
        public async void HasActiveUsers_GivenOrganisationWithActiveUser_TrueShouldBeReturned()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var organisationDataAccess = new OrganisationDataAccess(databaseWrapper.WeeeContext);
                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());

                databaseWrapper.WeeeContext.Organisations.Add(organisation);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                var organisationUser = new Domain.Organisation.OrganisationUser(Guid.Parse(databaseWrapper.Model.AspNetUsers.First().Id),
                    organisation.Id, UserStatus.Active);
                databaseWrapper.WeeeContext.OrganisationUsers.Add(organisationUser);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                var result = await organisationDataAccess.HasActiveUsers(organisation.Id);

                result.Should().BeTrue();
            }
        }

        public static IEnumerable<object[]> InactiveUserStatusData => new List<object[]>
        {
            new object[] {UserStatus.Pending},
            new object[] {UserStatus.Rejected},
            new object[] {UserStatus.Inactive}
        };
    
        [Theory]
        [MemberData(nameof(InactiveUserStatusData))]
        public async void HasActiveUsers_GivenOrganisationWithInactiveUser_FalseShouldBeReturned(UserStatus status)
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var organisationDataAccess = new OrganisationDataAccess(databaseWrapper.WeeeContext);
                var organisation = Domain.Organisation.Organisation.CreateSoleTrader(fixture.Create<string>());

                databaseWrapper.WeeeContext.Organisations.Add(organisation);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                var organisationUser = new Domain.Organisation.OrganisationUser(Guid.Parse(databaseWrapper.Model.AspNetUsers.First().Id), organisation.Id, status);
                databaseWrapper.WeeeContext.OrganisationUsers.Add(organisationUser);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                var result = await organisationDataAccess.HasActiveUsers(organisation.Id);

                result.Should().BeFalse();
            }
        }
    }
}
