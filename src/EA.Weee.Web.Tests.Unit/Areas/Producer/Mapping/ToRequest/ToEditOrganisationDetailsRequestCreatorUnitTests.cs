namespace EA.Weee.Web.Tests.Unit.Areas.Producer.Mapping.ToRequest
{
    using AutoFixture;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using EA.Weee.Tests.Core;
    using EA.Weee.Web.Areas.Producer.Mappings.ToRequest;
    using EA.Weee.Web.Areas.Producer.ViewModels;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class ToEditOrganisationDetailsRequestCreatorUnitTests : SimpleUnitTestBase
    {
        private readonly IMapper mapper;
        private readonly ToEditOrganisationDetailsRequestCreator requestCreator;

        public ToEditOrganisationDetailsRequestCreatorUnitTests()
        {
            mapper = A.Fake<IMapper>();
            requestCreator = new ToEditOrganisationDetailsRequestCreator(mapper);
        }

        [Fact]
        public void ViewModelToRequest_ShouldMapCorrectly()
        {
            // Arrange
            var viewModel = TestFixture.Create<EditOrganisationDetailsViewModel>();

            var mappedAddress = TestFixture.Create<AddressData>();

            A.CallTo(() => mapper.Map<ExternalAddressData, AddressData>(viewModel.Organisation.Address))
                .Returns(mappedAddress);

            // Act
            var result = requestCreator.ViewModelToRequest(viewModel);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<EditOrganisationDetailsRequest>();
            result.DirectRegistrantId.Should().Be(viewModel.DirectRegistrantId);
            result.CompanyName.Should().Be(viewModel.Organisation.CompanyName);
            result.TradingName.Should().Be(viewModel.Organisation.BusinessTradingName);
            result.BusinessAddressData.Should().Be(mappedAddress);
            result.EEEBrandNames.Should().BeEquivalentTo(viewModel.Organisation.EEEBrandNames);
        }
    }
}