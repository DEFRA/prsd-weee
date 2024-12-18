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

    public class ToServiceOfNoticeRequestCreatorUnitTests : SimpleUnitTestBase
    {
        private readonly IMapper mapper;
        private readonly ToServiceOfNoticeRequestCreator requestCreator;

        public ToServiceOfNoticeRequestCreatorUnitTests()
        {
            mapper = A.Fake<IMapper>();
            requestCreator = new ToServiceOfNoticeRequestCreator(mapper);
        }

        [Fact]
        public void ViewModelToRequest_ShouldMapCorrectly()
        {
            // Arrange
            var viewModel = TestFixture.Create<ServiceOfNoticeViewModel>();

            var mappedAddress = TestFixture.Create<AddressData>();

            A.CallTo(() => mapper.Map<ServiceOfNoticeAddressData, AddressData>(viewModel.Address))
                .Returns(mappedAddress);

            // Act
            var result = requestCreator.ViewModelToRequest(viewModel);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ServiceOfNoticeRequest>();
            result.DirectRegistrantId.Should().Be(viewModel.DirectRegistrantId);
            result.Address.Should().Be(mappedAddress);
        }
    }
}