namespace EA.Weee.DataAccess.Tests.Integration.DataAccess
{
    using System;
    using System.Linq;
    using AutoFixture;
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

                databaseWrapper.WeeeContext.Organisations.Add(organisation);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                databaseWrapper.WeeeContext.Organisations.Where(o => o.Id == organisation.Id).Should().NotBeEmpty();

                await organisationDataAccess.Delete(organisation.Id);

                databaseWrapper.WeeeContext.Organisations.Where(o => o.Id == organisation.Id).Should().BeEmpty();
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
    }
}
