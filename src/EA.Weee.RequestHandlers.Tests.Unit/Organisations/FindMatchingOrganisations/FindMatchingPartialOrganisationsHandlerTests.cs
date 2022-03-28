namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations.FindMatchingOrganisations
{
    using EA.Prsd.Core.Domain;
    using EA.Weee.RequestHandlers.Organisations.FindMatchingOrganisations.DataAccess;
    using EA.Weee.RequestHandlers.Organisations.FindMatchingPartialOrganisations;
    using EA.Weee.Requests.Organisations;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class FindMatchingPartialOrganisationsHandlerTests
    {
        private readonly DbContextHelper helper = new DbContextHelper();

        private readonly OrganisationHelper orgHelper = new OrganisationHelper();

        private readonly IFindMatchingOrganisationsDataAccess dataAccess;
        private readonly IUserContext userContext;

        public FindMatchingPartialOrganisationsHandlerTests()
        {
            dataAccess = A.Fake<IFindMatchingOrganisationsDataAccess>();
            userContext = A.Fake<IUserContext>();
        }

        [Fact]
        public async Task FindMatchingPartialOrganisationsHandler_Search_ReturnsOrganisationsReturned()
        {
            var org1 = orgHelper.GetOrganisationWithName("TEST Ltd");
            var org2 = orgHelper.GetOrganisationWithName("TEST  Limited");

            A.CallTo(() => dataAccess.GetOrganisationsByPartialSearchAsync(A<string>._, A<Guid>._))
                .Returns(new[]
                {
                    org1,
                    org2,
                });

            var strings = await FindMatchingPartialOrganisationsHandler().HandleAsync(new FindMatchingPartialOrganisations("test"));

            strings.Results.Count.Should().Be(2);
            strings.Results[0].Id.Should().Be(org1.Id);
            strings.Results[1].Id.Should().Be(org2.Id);
        }

        [Fact]
        public async Task FindMatchingPartialOrganisationsHandler_EmptySearch_ThrowsArgumentException()
        {
            Func<Task> act = FindMatchingPartialOrganisationsHandler().Awaiting(x => x.HandleAsync(new FindMatchingPartialOrganisations(string.Empty)));

            await act.Should().ThrowAsync<ArgumentException>();
        }

        private FindMatchingPartialOrganisationsHandler FindMatchingPartialOrganisationsHandler()
        {
            return new FindMatchingPartialOrganisationsHandler(dataAccess, userContext);
        }
    }
}
