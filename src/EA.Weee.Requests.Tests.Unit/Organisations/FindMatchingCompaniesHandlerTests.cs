namespace EA.Weee.Requests.Tests.Unit.Organisations
{
    using System;
    using System.Collections.Generic;
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
        private readonly OrganisationType anyType = OrganisationType.RegisteredCompany;
        private readonly OrganisationStatus anyStatus = OrganisationStatus.Incomplete;
     
        private readonly DbContextHelper helper = new DbContextHelper();

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

        private Organisation GetOrganisationWithName(string name)
        {
            var organisation = new Organisation(name, anyType, anyStatus);

            organisation.OrganisationAddress = GetAddress();

            var properties = typeof(Organisation).GetProperties();

            foreach (var propertyInfo in properties)
            {
                if (propertyInfo.Name.Equals("Id", StringComparison.InvariantCultureIgnoreCase))
                {
                    var baseProperty = typeof(Organisation).BaseType.GetProperty(propertyInfo.Name);

                    baseProperty.SetValue(organisation, Guid.NewGuid(), null);

                    break;
                }
            }

            return organisation;
        }

        private Address GetAddress()
        {
            return new Address("1", "street", "Woking", "Hampshire", "GU21 5EE", "United Kingdom", "12345678", "test@co.uk");
        }

        [Fact]
        public async Task FindMatchingOrganisationsHandler_WithLtdCompany_OneMatches()
        {
            var organisations = helper.GetAsyncEnabledDbSet(new[]
            {
                GetOrganisationWithName("SFW Ltd"),
                GetOrganisationWithName("swf"),
                GetOrganisationWithName("mfw"),
                GetOrganisationWithName("mfi Kitchens and Showrooms Ltd"),
                GetOrganisationWithName("SEPA England"),
                GetOrganisationWithName("Tesco Recycling")
            });

            var context = A.Fake<WeeeContext>();

            A.CallTo(() => context.Organisations).Returns(organisations);

            var handler = new FindMatchingOrganisationsHandler(context);

            var strings = await handler.HandleAsync(new FindMatchingOrganisations("sfw"));

            Assert.Equal(1, strings.Count);
        }

        [Fact]
        public async Task FindMatchingOrganisationsHandler_WithLtdAndLimitedCompany_TwoMatches()
        {
            var organisations = helper.GetAsyncEnabledDbSet(new[]
            {
                GetOrganisationWithName("SFW Ltd"),
                GetOrganisationWithName("SFW  Limited")
            });

            var context = A.Fake<WeeeContext>();

            A.CallTo(() => context.Organisations).Returns(organisations);

            var handler = new FindMatchingOrganisationsHandler(context);

            var strings = await handler.HandleAsync(new FindMatchingOrganisations("sfw"));

            Assert.Equal(2, strings.Count);
        }

        [Fact]
        public async Task FindMatchingOrganisationsHandler_SearchTermContainsThe_ReturnsMatchingResults()
        {
            var organisations = helper.GetAsyncEnabledDbSet(new[]
            {
                GetOrganisationWithName("Environment Agency"),
                GetOrganisationWithName("Enivronent Agency")
            });

            var context = A.Fake<WeeeContext>();

            A.CallTo(() => context.Organisations).Returns(organisations);

            var handler = new FindMatchingOrganisationsHandler(context);

            var results = await handler.HandleAsync(new FindMatchingOrganisations("THe environment agency"));

            Assert.Equal(2, results.Count);
        }

        [Fact]
        public async Task FindMatchingOrganisationsHandler_DataContainsThe_ReturnsMatchingResults()
        {
            var data = new[]
            {
                GetOrganisationWithName("THE  Environemnt Agency"),
                GetOrganisationWithName("THE Environemnt Agency"),
                GetOrganisationWithName("Environment Agency")
            };

            var organisations = helper.GetAsyncEnabledDbSet(data);

            var context = A.Fake<WeeeContext>();

            A.CallTo(() => context.Organisations).Returns(organisations);

            var handler = new FindMatchingOrganisationsHandler(context);

            var results = await handler.HandleAsync(new FindMatchingOrganisations("Environment Agency"));

            Assert.Equal(data.Length, results.Count);
        }

        [Fact]
        public async Task FindMatchingOrganisationsHandler_AllDataMatches_ReturnedStringsMatchInputDataWithCase()
        {
            var names = new[] { "Environment Agency", "Environemnt Agincy" };

            var data = names.Select(GetOrganisationWithName).ToArray();

            var organisations = helper.GetAsyncEnabledDbSet(data);

            var context = A.Fake<WeeeContext>();

            A.CallTo(() => context.Organisations).Returns(organisations);

            var handler = new FindMatchingOrganisationsHandler(context);

            var results = await handler.HandleAsync(new FindMatchingOrganisations("Environment Agency"));

            Assert.Equal(names, results.Select(r => r.Name));
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
                helper.GetAsyncEnabledDbSet(namesWithDistances.Select(n => GetOrganisationWithName(n.Key)).ToArray());

            var context = A.Fake<WeeeContext>();

            A.CallTo(() => context.Organisations).Returns(organisations);

            var handler = new FindMatchingOrganisationsHandler(context);

            var results = await handler.HandleAsync(new FindMatchingOrganisations(searchTerm));

            Assert.Equal(namesWithDistances.OrderBy(n => n.Value).Select(n => n.Key), results.Select(r => r.Name));
        }
    }
}