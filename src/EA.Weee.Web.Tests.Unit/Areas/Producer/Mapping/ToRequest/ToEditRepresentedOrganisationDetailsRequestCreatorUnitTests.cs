namespace EA.Weee.Web.Tests.Unit.Areas.Producer.Mapping.ToRequest
{
    using AutoFixture;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using EA.Weee.Tests.Core;
    using EA.Weee.Web.Areas.Producer.Mappings.ToRequest;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class ToEditRepresentedOrganisationDetailsRequestCreatorUnitTests : SimpleUnitTestBase
    {
        private readonly IMapper mapper;
        private readonly ToEditRepresentedOrganisationDetailsRequestCreator requestCreator;

        public ToEditRepresentedOrganisationDetailsRequestCreatorUnitTests()
        {
            mapper = A.Fake<IMapper>();
            requestCreator = new ToEditRepresentedOrganisationDetailsRequestCreator();
        }

        [Fact]
        public void ViewModelToRequest_ShouldMapCorrectly()
        {
            // Arrange
            var viewModel = TestFixture.Create<RepresentingCompanyDetailsViewModel>();

            // Act
            var result = requestCreator.ViewModelToRequest(viewModel);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<RepresentedOrganisationDetailsRequest>();
            result.DirectRegistrantId.Should().Be(viewModel.DirectRegistrantId);
            result.BusinessTradingName.Should().Be(viewModel.BusinessTradingName);
            result.Address.Should().Be(viewModel.Address);
        }
    }
}