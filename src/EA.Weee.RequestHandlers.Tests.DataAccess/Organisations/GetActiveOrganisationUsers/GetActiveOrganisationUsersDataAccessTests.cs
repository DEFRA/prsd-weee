namespace EA.Weee.RequestHandlers.Tests.DataAccess.Organisations.GetActiveOrganisationUsers
{
    using EA.Weee.DataAccess;
    using EA.Weee.RequestHandlers.Organisations.GetActiveOrganisationUsers.DataAccess;
    using EA.Weee.Tests.Core;
    using EA.Weee.Tests.Core.Model;
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

    public class GetActiveOrganisationUsersDataAccessTests
    {
        private readonly WeeeContext context;

        public GetActiveOrganisationUsersDataAccessTests()
        {
            context = A.Fake<WeeeContext>();
        }

        [Fact]
        public async void FetchActiveOrganisationUsers_GivenOrganisationId_ReturnsActiveUsersLinkedToOrganisations()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                ModelHelper helper = new ModelHelper(database.Model);
                DomainHelper domainHelper = new DomainHelper(database.WeeeContext);

                var organisation = helper.CreateOrganisation();
                var organisation2 = helper.CreateOrganisation();

                var user = helper.CreateOrganisationUser(organisation, "user1@sfwltd.co.uk", 2);
                var user2 = helper.CreateOrganisationUser(organisation, "user2@sfwltd.co.uk", 1);
                var user3 = helper.CreateOrganisationUser(organisation, "user3@sfwltd.co.uk", 2);

                database.Model.SaveChanges();

                var dataAccess = new GetActiveOrganisationUsersDataAccess(database.WeeeContext);

                var results = await dataAccess.FetchActiveOrganisationUsers(organisation.Id);

                foreach (var activeUser in results)
                {
                    activeUser.UserStatus.Value.Should().Be(2);
                    activeUser.OrganisationId.Should().Be(organisation.Id);
                }
            }
        }

        [Fact]
        public async void FetchActiveOrganisationUsers_GivenOrganisationId_UsersShouldBeOrganisedByEmails()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                ModelHelper helper = new ModelHelper(database.Model);
                DomainHelper domainHelper = new DomainHelper(database.WeeeContext);

                var organisation = helper.CreateOrganisation();
                var user = helper.CreateOrganisationUser(organisation, "a@sfwltd.co.uk", 2);
                var user2 = helper.CreateOrganisationUser(organisation, "b@sfwltd.co.uk", 2);

                database.Model.SaveChanges();

                var dataAccess = new GetActiveOrganisationUsersDataAccess(database.WeeeContext);

                var results = await dataAccess.FetchActiveOrganisationUsers(organisation.Id);

                results.Should().BeInAscendingOrder(x => x.User.Email);
            }
        }
    }
}
