namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations.FindMatchingOrganisations
{
    using Domain.Organisation;
    using FakeItEasy;
    using Prsd.Core;
    using Prsd.Core.Domain;
    using RequestHandlers.Organisations.FindMatchingOrganisations;
    using RequestHandlers.Organisations.FindMatchingOrganisations.DataAccess;
    using Requests.Organisations;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Weee.Tests.Core;
    using Xunit;

    public class FindMatchingOrganisationsHandlerTests
    {
        private readonly string companyRegistrationNumber = "AB123456";

        private readonly DbContextHelper helper = new DbContextHelper();

        private readonly OrganisationHelper orgHelper = new OrganisationHelper();

        private readonly IFindMatchingOrganisationsDataAccess dataAccess;
        private readonly IUserContext userContext;

        public FindMatchingOrganisationsHandlerTests()
        {
            dataAccess = A.Fake<IFindMatchingOrganisationsDataAccess>();
            userContext = A.Fake<IUserContext>();
        }

        [Theory]
        [InlineData("", "", 0)]
        [InlineData("", "Bee", 3)]
        [InlineData("Bee", "Bee", 0)]
        [InlineData("Bee", "Bee ", 1)]
        [InlineData("Bee", "Bea", 1)]
        [InlineData("Bee", "Bae", 1)]
        [InlineData("Bee", "eae", 2)]
        [InlineData("Bee", "eaa", 3)]
        [InlineData("Bee", "bee", 0)]
        [InlineData("Bee Tee Tower", "BeeTeeTower", 2)]
        [InlineData("BeeTeeTower", "Bee Tee Tower", 2)]
        [InlineData("Longererer String", "Small String", 10)]
        [InlineData("Small String", "Longererer String", 10)]
        public void StringSearch_CalculateLevenshteinDistance_ReturnsExpectedResult(string s1, string s2, int result)
        {
            Assert.Equal(result, StringSearch.CalculateLevenshteinDistance(s1, s2));
        }

        [Fact]
        public async Task FindMatchingOrganisationsHandler_WithLtdCompany_OneMatches()
        {
            A.CallTo(() => dataAccess.GetOrganisationsBySimpleSearchTerm(A<string>._, A<Guid>._))
                .Returns(new[]
                {
                    orgHelper.GetOrganisationWithName("Test Ltd"),
                    orgHelper.GetOrganisationWithName("tset"),
                    orgHelper.GetOrganisationWithName("mfw"),
                    orgHelper.GetOrganisationWithName("mfi Kitchens and Showrooms Ltd"),
                    orgHelper.GetOrganisationWithName("SEPA England"),
                    orgHelper.GetOrganisationWithName("Tesco Recycling")
                });

            var strings = await FindMatchingOrganisationsHandler().HandleAsync(new FindMatchingOrganisations("Test"));

            Assert.Equal(1, strings.Results.Count());
        }

        [Fact]
        public async Task FindMatchingOrganisationsHandler_WithLtdAndLimitedCompany_TwoMatches()
        {
            A.CallTo(() => dataAccess.GetOrganisationsBySimpleSearchTerm(A<string>._, A<Guid>._))
                .Returns(new[]
                {
                    orgHelper.GetOrganisationWithName("TEST Ltd"),
                    orgHelper.GetOrganisationWithName("TEST  Limited")
                });

            var strings = await FindMatchingOrganisationsHandler().HandleAsync(new FindMatchingOrganisations("test"));

            Assert.Equal(2, strings.Results.Count());
        }

        [Fact]
        public async Task FindMatchingOrganisationsHandler_SearchTermContainsThe_ReturnsMatchingResults()
        {
            A.CallTo(() => dataAccess.GetOrganisationsBySimpleSearchTerm(A<string>._, A<Guid>._))
                .Returns(new[]
                {
                    orgHelper.GetOrganisationWithName("Environment Agency"),
                    orgHelper.GetOrganisationWithName("Enivronent Agency")
                });

            var results = await FindMatchingOrganisationsHandler().HandleAsync(new FindMatchingOrganisations("THe environment agency"));

            Assert.Equal(2, results.Results.Count());
        }

        [Fact]
        public async Task FindMatchingOrganisationsHandler_DataContainsThe_ReturnsMatchingResults()
        {
            var data = new[]
            {
                orgHelper.GetOrganisationWithName("THE  Environemnt Agency"),
                orgHelper.GetOrganisationWithName("THE Environemnt Agency"),
                orgHelper.GetOrganisationWithName("Environment Agency")
            };

            A.CallTo(() => dataAccess.GetOrganisationsBySimpleSearchTerm(A<string>._, A<Guid>._))
                .Returns(data);

            var results = await FindMatchingOrganisationsHandler().HandleAsync(new FindMatchingOrganisations("Environment Agency"));

            Assert.Equal(data.Length, results.Results.Count());
        }

        [Fact]
        public async Task FindMatchingOrganisationsHandler_AllDataMatches_ReturnedStringsMatchInputDataWithCase()
        {
            var names = new[] { "Environment Agency", "Environemnt Agincy" };
            var data = names.Select(orgHelper.GetOrganisationWithName).ToArray();

            A.CallTo(() => dataAccess.GetOrganisationsBySimpleSearchTerm(A<string>._, A<Guid>._))
                .Returns(data);

            var results = await FindMatchingOrganisationsHandler().HandleAsync(new FindMatchingOrganisations("Environment Agency"));

            Assert.Equal(names, results.Results.Select(r => r.DisplayName).ToArray());
        }

        [Fact]
        public async Task FindMatchingOrganisationsHandler_AllDataMatches_ReturnsDataOrderedByEditDistance()
        {
            var searchTerm = "bee keepers";

            var namesWithDistances = new[]
            {
                new KeyValuePair<string, int>("THE Bee Keepers Limited", 0),
                new KeyValuePair<string, int>("Bee Keeperes", 1),
                new KeyValuePair<string, int>("BeeKeeprs", 2)
            };

            var organisations =
                namesWithDistances.Select(n => orgHelper.GetOrganisationWithName(n.Key)).ToArray();

            A.CallTo(() => dataAccess.GetOrganisationsBySimpleSearchTerm(A<string>._, A<Guid>._))
                .Returns(organisations);

            var results = await FindMatchingOrganisationsHandler().HandleAsync(new FindMatchingOrganisations(searchTerm));

            Assert.Equal(namesWithDistances.OrderBy(n => n.Value).Select(n => n.Key),
                results.Results.Select(r => r.DisplayName));
        }

        [Fact]
        public async Task FindMatchingOrganisationsHandler_IncludingSearchOnTradingName_ReturnsMatchingResults()
        {
            var data = new[]
            {
                orgHelper.GetOrganisationWithDetails("THE  Environemnt Agency", null, companyRegistrationNumber,
                    OrganisationType.RegisteredCompany, OrganisationStatus.Complete),
                orgHelper.GetOrganisationWithDetails("THE  Environemnt Agency", "THE Evironemnt Agency",
                    companyRegistrationNumber, OrganisationType.RegisteredCompany, OrganisationStatus.Complete),
                orgHelper.GetOrganisationWithDetails(null, "THE Environemnt Agency", companyRegistrationNumber,
                    OrganisationType.SoleTraderOrIndividual, OrganisationStatus.Complete),
                orgHelper.GetOrganisationWithDetails(null, "Environment Agency", companyRegistrationNumber,
                    OrganisationType.Partnership, OrganisationStatus.Complete)
            };

            A.CallTo(() => dataAccess.GetOrganisationsBySimpleSearchTerm(A<string>._, A<Guid>._))
                .Returns(data);

            var results = await FindMatchingOrganisationsHandler().HandleAsync(new FindMatchingOrganisations("Environment Agency"));

            Assert.Equal(data.Length, results.Results.Count());
        }

        [Fact]
        public async Task FindMatchingOrganisationsHandler_ReturnsMatchingResults()
        {
            const string IdenticalToQuery = "Environment Agency";
            const string CloseToQuery = "Enviroment Agency";
            const string QuiteDifferentToQuery = "Environmt Agecy";
            const string CompletelyUnlikeQuery = "I am a lamppost";

            var data = new[]
            {
                orgHelper.GetOrganisationWithDetails(IdenticalToQuery, null, companyRegistrationNumber,
                    OrganisationType.RegisteredCompany, OrganisationStatus.Complete),
                orgHelper.GetOrganisationWithDetails(CloseToQuery, null, companyRegistrationNumber,
                    OrganisationType.RegisteredCompany, OrganisationStatus.Complete),
                orgHelper.GetOrganisationWithDetails(QuiteDifferentToQuery, null, companyRegistrationNumber,
                    OrganisationType.RegisteredCompany, OrganisationStatus.Complete),
                orgHelper.GetOrganisationWithDetails(CompletelyUnlikeQuery, null, companyRegistrationNumber,
                    OrganisationType.RegisteredCompany, OrganisationStatus.Complete)
            };

            A.CallTo(() => dataAccess.GetOrganisationsBySimpleSearchTerm(A<string>._, A<Guid>._))
                .Returns(data);

            var handler = FindMatchingOrganisationsHandler();

            var orgs = await handler.HandleAsync(new FindMatchingOrganisations("Environment Agency"));
           
            // handler sorts by distance
            Assert.Equal(IdenticalToQuery, orgs.Results[0].DisplayName);
            Assert.Equal(CloseToQuery, orgs.Results[1].DisplayName);
            Assert.Equal(QuiteDifferentToQuery, orgs.Results[2].DisplayName);
        }

        private FindMatchingOrganisationsHandler FindMatchingOrganisationsHandler()
        {
            return new FindMatchingOrganisationsHandler(dataAccess, userContext);
        }
    }
}