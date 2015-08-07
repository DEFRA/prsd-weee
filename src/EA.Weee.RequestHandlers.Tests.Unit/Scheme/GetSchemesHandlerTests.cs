namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.PCS;
    using DataAccess;
    using Domain.Organisation;
    using Domain.Scheme;
    using FakeItEasy;
    using RequestHandlers.Mappings;
    using RequestHandlers.PCS;
    using RequestHandlers.Tests.Unit.Helpers;
    using Requests.Scheme;
    using Xunit;

    public class GetSchemesHandlerTests
    {
        private readonly DbContextHelper helper = new DbContextHelper();
        private readonly OrganisationHelper orgHelper = new OrganisationHelper();

        [Fact]
        public async Task GetSchemesHandler_OnlyIncompleteOrganisations_EmptyListReturned()
        {
            const string IncompleteOrganisationName = "A";

            var incompleteScheme = MakeFakeSchemeWithOrganisationDetails(IncompleteOrganisationName, OrganisationStatus.Incomplete);

            var result = await RunHandler(incompleteScheme);

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetSchemesHandler_TwoCompleteOrganisationAndOneIncompleteOrganisation_TwoCompleteSchemesReturned()
        {
            const string IncompleteOrganisationName = "A";
            const string CompleteOrganisationOneName = "B";
            const string CompleteOrganisationTwoName = "C";

            var incompleteScheme = MakeFakeSchemeWithOrganisationDetails(IncompleteOrganisationName, OrganisationStatus.Incomplete);
            var completeSchemeOne = MakeFakeSchemeWithOrganisationDetails(CompleteOrganisationOneName, OrganisationStatus.Complete);
            var completeSchemeTwo = MakeFakeSchemeWithOrganisationDetails(CompleteOrganisationTwoName, OrganisationStatus.Complete);

            var result = await RunHandler(incompleteScheme, completeSchemeOne, completeSchemeTwo);

            Assert.Equal(2, result.Count);
            Assert.True(result.Exists(sd => sd.Name == CompleteOrganisationOneName));
            Assert.True(result.Exists(sd => sd.Name == CompleteOrganisationTwoName));
        }

        [Fact]
        public async Task GetSchemesHandler_ResultsOrderedByNameAscending()
        {
            const string FirstPositionName = "A";
            const string SecondPositionName = "B";
            const string ThirdPositionName = "C";

            var firstScheme = MakeFakeSchemeWithOrganisationDetails(FirstPositionName, OrganisationStatus.Complete);
            var secondScheme = MakeFakeSchemeWithOrganisationDetails(SecondPositionName, OrganisationStatus.Complete);
            var thirdScheme = MakeFakeSchemeWithOrganisationDetails(ThirdPositionName, OrganisationStatus.Complete);

            var result = await RunHandler(thirdScheme, firstScheme, secondScheme); // different order

            Assert.Equal(FirstPositionName, result[0].Name);
            Assert.Equal(SecondPositionName, result[1].Name);
            Assert.Equal(ThirdPositionName, result[2].Name);
        }

        private async Task<List<SchemeData>> RunHandler(params Scheme[] schemes)
        {
            var context = MakeContextWithSchemes(schemes);
            var handler = new GetSchemesHandler(context, new SchemeMap());
            return await handler.HandleAsync(new GetSchemes());
        }

        private Scheme MakeFakeSchemeWithOrganisationDetails(string name, OrganisationStatus status)
        {
            var scheme = A.Fake<Scheme>();
            var organisation = MakeOrganisationWithDetails(name, status);
            A.CallTo(() => scheme.Organisation).Returns(organisation);
            return scheme;
        }

        private Organisation MakeOrganisationWithDetails(string name, OrganisationStatus status)
        {
            return orgHelper.GetOrganisationWithDetails(
                name,
                name,
                "12345678",
                OrganisationType.SoleTraderOrIndividual,
                status);
        }

        private WeeeContext MakeContextWithSchemes(params Scheme[] schemes)
        {
            var context = A.Fake<WeeeContext>();
            var schemesDbSet = helper.GetAsyncEnabledDbSet(schemes);
            A.CallTo(() => context.Schemes).Returns(schemesDbSet);
            return context;
        }
    }
}
