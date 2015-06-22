namespace EA.Weee.Requests.Tests.Unit.Organisations
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain;
    using FakeItEasy;
    using Helpers;
    using Prsd.Core;
    using RequestHandlers.Organisations;
    using Requests.Organisations;
    using Xunit;

    public class FindMatchingCompaniesHandlerTests
    {
        private readonly string companyRegistrationNumber = "AB123456";
     
        private readonly DbContextHelper helper = new DbContextHelper();

        private readonly OrganisationHelper orgHelper = new OrganisationHelper();

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
            var organisations = helper.GetAsyncEnabledDbSet(new[]
            {
                orgHelper.GetOrganisationWithName("SFW Ltd"),
                orgHelper.GetOrganisationWithName("swf"),
                orgHelper.GetOrganisationWithName("mfw"),
                orgHelper.GetOrganisationWithName("mfi Kitchens and Showrooms Ltd"),
                orgHelper.GetOrganisationWithName("SEPA England"),
                orgHelper.GetOrganisationWithName("Tesco Recycling")
            });

            var context = A.Fake<WeeeContext>();

            A.CallTo(() => context.Organisations).Returns(organisations);

            var handler = new FindMatchingOrganisationsHandler(context);

            var strings = await handler.HandleAsync(new FindMatchingOrganisations("sfw"));

            Assert.Equal(1, strings.Results.Count());
        }

        [Fact]
        public async Task FindMatchingOrganisationsHandler_WithLtdAndLimitedCompany_TwoMatches()
        {
            var organisations = helper.GetAsyncEnabledDbSet(new[]
            {
                orgHelper.GetOrganisationWithName("SFW Ltd"),
                orgHelper.GetOrganisationWithName("SFW  Limited")
            });

            var context = A.Fake<WeeeContext>();

            A.CallTo(() => context.Organisations).Returns(organisations);

            var handler = new FindMatchingOrganisationsHandler(context);

            var strings = await handler.HandleAsync(new FindMatchingOrganisations("sfw"));

            Assert.Equal(2, strings.Results.Count());
        }

        [Fact]
        public async Task FindMatchingOrganisationsHandler_SearchTermContainsThe_ReturnsMatchingResults()
        {
            var organisations = helper.GetAsyncEnabledDbSet(new[]
            {
                orgHelper.GetOrganisationWithName("Environment Agency"),
                orgHelper.GetOrganisationWithName("Enivronent Agency")
            });

            var context = A.Fake<WeeeContext>();

            A.CallTo(() => context.Organisations).Returns(organisations);

            var handler = new FindMatchingOrganisationsHandler(context);

            var results = await handler.HandleAsync(new FindMatchingOrganisations("THe environment agency"));

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

            var organisations = helper.GetAsyncEnabledDbSet(data);

            var context = A.Fake<WeeeContext>();

            A.CallTo(() => context.Organisations).Returns(organisations);

            var handler = new FindMatchingOrganisationsHandler(context);

            var results = await handler.HandleAsync(new FindMatchingOrganisations("Environment Agency"));

            Assert.Equal(data.Length, results.Results.Count());
        }

        [Fact]
        public async Task FindMatchingOrganisationsHandler_AllDataMatches_ReturnedStringsMatchInputDataWithCase()
        {
            var names = new[] { "Environment Agency", "Environemnt Agincy" };

            var data = names.Select(orgHelper.GetOrganisationWithName).ToArray();

            var organisations = helper.GetAsyncEnabledDbSet(data);

            var context = A.Fake<WeeeContext>();

            A.CallTo(() => context.Organisations).Returns(organisations);

            var handler = new FindMatchingOrganisationsHandler(context);

            var results = await handler.HandleAsync(new FindMatchingOrganisations("Environment Agency"));

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
                helper.GetAsyncEnabledDbSet(namesWithDistances.Select(n => orgHelper.GetOrganisationWithName(n.Key)).ToArray());

            var context = A.Fake<WeeeContext>();

            A.CallTo(() => context.Organisations).Returns(organisations);

            var handler = new FindMatchingOrganisationsHandler(context);

            var results = await handler.HandleAsync(new FindMatchingOrganisations(searchTerm));

            Assert.Equal(namesWithDistances.OrderBy(n => n.Value).Select(n => n.Key), results.Results.Select(r => r.DisplayName));
        }

        [Fact]
        public async Task FindMatchingOrganisationsHandler_IncludingSearchOnTradingName_ReturnsMatchingResults()
        {
            var data = new[]
            {
                orgHelper.GetOrganisationWithDetails("THE  Environemnt Agency", null, companyRegistrationNumber, Domain.OrganisationType.RegisteredCompany, OrganisationStatus.Approved),
                orgHelper.GetOrganisationWithDetails("THE  Environemnt Agency", "THE Evironemnt Agency", companyRegistrationNumber, Domain.OrganisationType.RegisteredCompany, OrganisationStatus.Approved),
                orgHelper.GetOrganisationWithDetails(null, "THE Environemnt Agency", companyRegistrationNumber, Domain.OrganisationType.SoleTraderOrIndividual, OrganisationStatus.Approved),
                orgHelper.GetOrganisationWithDetails(null, "Environment Agency", companyRegistrationNumber, Domain.OrganisationType.Partnership, OrganisationStatus.Approved)
            };

            var organisations = helper.GetAsyncEnabledDbSet(data);

            var context = A.Fake<WeeeContext>();

            A.CallTo(() => context.Organisations).Returns(organisations);

            var handler = new FindMatchingOrganisationsHandler(context);

            var results = await handler.HandleAsync(new FindMatchingOrganisations("Environment Agency"));

            Assert.Equal(data.Length, results.Results.Count());
        }

        [Fact]
        public async Task FindMatchingOrganisationsHandler_Paged_ReturnsMatchingResults()
        {
            const string IdenticalToQuery = "Environment Agency";
            const string CloseToQuery = "Enviroment Agency";
            const string QuiteDifferentToQuery = "Environmt Agecy";
            const string CompletelyUnlikeQuery = "I am a lamppost";

            var data = new[]
            {
                orgHelper.GetOrganisationWithDetails(IdenticalToQuery, null, companyRegistrationNumber, Domain.OrganisationType.RegisteredCompany, OrganisationStatus.Approved),
                orgHelper.GetOrganisationWithDetails(CloseToQuery, null, companyRegistrationNumber, Domain.OrganisationType.RegisteredCompany, OrganisationStatus.Approved),
                orgHelper.GetOrganisationWithDetails(QuiteDifferentToQuery, null, companyRegistrationNumber, Domain.OrganisationType.RegisteredCompany, OrganisationStatus.Approved),
                orgHelper.GetOrganisationWithDetails(CompletelyUnlikeQuery, null, companyRegistrationNumber, Domain.OrganisationType.RegisteredCompany, OrganisationStatus.Approved)
            };

            var organisations = helper.GetAsyncEnabledDbSet(data);

            var context = A.Fake<WeeeContext>();

            A.CallTo(() => context.Organisations).Returns(organisations);

            var handler = new FindMatchingOrganisationsHandler(context);

            const int PageSize = 2;

            var firstPage = await handler.HandleAsync(new FindMatchingOrganisations("Environment Agency", 1, PageSize));
            var secondPage = await handler.HandleAsync(new FindMatchingOrganisations("Environment Agency", 2, PageSize));
            var thirdEmptyPage = await handler.HandleAsync(new FindMatchingOrganisations("Environment Agency", 3, PageSize));

            Assert.Equal(PageSize, firstPage.Results.Count());
            Assert.Equal(PageSize - 1, secondPage.Results.Count()); // we aren't expecting to see the one named CompletelyUnlikeQuery
            Assert.Equal(0, thirdEmptyPage.Results.Count());

            // handler sorts by distance
            Assert.Equal(IdenticalToQuery, firstPage.Results[0].DisplayName);
            Assert.Equal(CloseToQuery, firstPage.Results[1].DisplayName);

            Assert.Equal(QuiteDifferentToQuery, secondPage.Results[0].DisplayName);
        }

        [Fact]
        public async Task FindMatchingOrganisationsHandler_SearchMightIncludeIncompleteOrgs_OnlyShowComplete()
        {
            const string CompleteName   = "Environment Agency 1";
            const string IncompleteName = "Environment Agency 2";

            var data = new[]
            {
                orgHelper.GetOrganisationWithDetails(CompleteName,   null, companyRegistrationNumber, Domain.OrganisationType.RegisteredCompany, OrganisationStatus.Approved),
                orgHelper.GetOrganisationWithDetails(IncompleteName, null, companyRegistrationNumber, Domain.OrganisationType.RegisteredCompany, OrganisationStatus.Incomplete)
            };

            var organisations = helper.GetAsyncEnabledDbSet(data);

            var context = A.Fake<WeeeContext>();

            A.CallTo(() => context.Organisations).Returns(organisations);

            var handler = new FindMatchingOrganisationsHandler(context);

            var results = await handler.HandleAsync(new FindMatchingOrganisations("Environment Agency"));

            Assert.Equal(1, results.Results.Count());
            Assert.Equal(CompleteName, results.Results[0].DisplayName);
        }
    }
}