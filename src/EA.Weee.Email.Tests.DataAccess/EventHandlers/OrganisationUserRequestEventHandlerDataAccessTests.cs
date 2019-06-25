namespace EA.Weee.Email.Tests.DataAccess.EventHandlers
{
    using EA.Weee.Email.EventHandlers;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;
    using DatabaseWrapper = Weee.Tests.Core.Model.DatabaseWrapper;
    using DomainHelper = Weee.Tests.Core.DomainHelper;
    using ModelHelper = Weee.Tests.Core.Model.ModelHelper;

    public class OrganisationUserRequestEventHandlerDataAccessTests
    {
        [Fact]
        public async Task FetchUser_ReturnsSpecifiedUser()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(database.Model);
                DomainHelper domainHelper = new DomainHelper(database.WeeeContext);

                var user = helper.CreateUser("user1@civica.co.uk", Weee.Tests.Core.Model.IdType.Guid);
                var user2 = helper.CreateUser("user2@civica.co.uk", Weee.Tests.Core.Model.IdType.Guid);

                database.Model.SaveChanges();

                var dataAccess = new OrganisationUserRequestEventHandlerDataAccess(database.WeeeContext);

                // Act
                var result = await dataAccess.FetchUser(user.Id);

                // Assert
                result.Email.Should().Be(user.UserName);
            }
        }

        [Fact]
        public async Task FetchActiveOrganisationUsers_ReturnsSpecifiedUsers()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(database.Model);
                DomainHelper domainHelper = new DomainHelper(database.WeeeContext);

                var organisation = helper.CreateOrganisation();
                var organisationUser = helper.CreateOrganisationUser(organisation, "user1@civica.co.uk", 2);

                var organisation2 = helper.CreateOrganisation();
                var organisationUser2 = helper.CreateOrganisationUser(organisation2, "user2@civica.co.uk", 2);

                database.Model.SaveChanges();

                var dataAccess = new OrganisationUserRequestEventHandlerDataAccess(database.WeeeContext);

                // Act
                var results = await dataAccess.FetchActiveOrganisationUsers(organisation.Id);
                // Assert
                foreach (var result in results)
                {
                    result.User.Email.Should().Be(organisationUser.AspNetUser.Email);
                }
            }
        }

        [Fact]
        public async Task FetchOrganisation_ReturnsSpecifiedOrganisation()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(database.Model);
                DomainHelper domainHelper = new DomainHelper(database.WeeeContext);

                var organisation = helper.CreateOrganisation();
                organisation.Name = "Organisation";

                database.Model.SaveChanges();

                var dataAccess = new OrganisationUserRequestEventHandlerDataAccess(database.WeeeContext);

                // Act
                var result = await dataAccess.FetchOrganisation(organisation.Id);
                // Assert
                result.Name.Should().Be(organisation.Name);
            }
        }
    }
}
