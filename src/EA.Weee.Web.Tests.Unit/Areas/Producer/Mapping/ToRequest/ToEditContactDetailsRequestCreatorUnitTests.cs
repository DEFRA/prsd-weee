namespace EA.Weee.Web.Tests.Unit.Areas.Producer.Mapping.ToRequest
{
    using AutoFixture;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.Shared;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using EA.Weee.Tests.Core;
    using EA.Weee.Web.Areas.Producer.Mappings.ToRequest;
    using EA.Weee.Web.Areas.Producer.ViewModels;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class ToEditContactDetailsRequestCreatorUnitTests : SimpleUnitTestBase
    {
        private readonly IMapper mapper;
        private readonly ToEditContactDetailsRequestCreator requestCreator;

        public ToEditContactDetailsRequestCreatorUnitTests()
        {
            mapper = A.Fake<IMapper>();
            requestCreator = new ToEditContactDetailsRequestCreator(mapper);
        }

        [Fact]
        public void ViewModelToRequest_ShouldMapCorrectly()
        {
            // Arrange
            var viewModel = TestFixture.Create<EditContactDetailsViewModel>();

            var mappedAddress = TestFixture.Create<AddressData>();

            A.CallTo(() => mapper.Map<AddressPostcodeRequiredData, AddressData>(viewModel.ContactDetails.AddressData))
                .Returns(mappedAddress);

            // Act
            var result = requestCreator.ViewModelToRequest(viewModel);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<EditContactDetailsRequest>();
            result.DirectRegistrantId.Should().Be(viewModel.DirectRegistrantId);
            result.ContactData.FirstName.Should().Be(viewModel.ContactDetails.FirstName);
            result.ContactData.LastName.Should().Be(viewModel.ContactDetails.LastName);
            result.ContactData.Position.Should().Be(viewModel.ContactDetails.Position);
            result.AddressData.Should().Be(mappedAddress);
        }
    }
}