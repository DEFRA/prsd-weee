namespace EA.Weee.Web.Tests.Unit.Areas.Producer.Mapping.ToViewModel
{
    using AutoFixture;
    using AutoFixture.AutoFakeItEasy;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Shared;
    using EA.Weee.Web.Areas.Producer.Mappings.ToViewModel;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class EditContactDetailsMapTests
    {
        private readonly IFixture fixture;
        private readonly IMapper mapper;
        private readonly EditContactDetailsMap map;

        public EditContactDetailsMapTests()
        {
            fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            mapper = fixture.Freeze<IMapper>();
            map = new EditContactDetailsMap(mapper);
        }

        [Fact]
        public void Map_ShouldMaphighLevelSourceFields()
        {
            // Arrange
            var source = fixture.Create<SmallProducerSubmissionData>();

            // Act
            var result = map.Map(source);

            // Assert
            result.DirectRegistrantId.Should().Be(source.DirectRegistrantId);
            result.OrganisationId.Should().Be(source.OrganisationData.Id);
            result.HasAuthorisedRepresentitive.Should().Be(source.HasAuthorisedRepresentitive);
        }

        [Fact]
        public void Map_ShouldUseCurrentSubmissionDataWhenAvailable()
        {
            // Arrange
            var source = fixture.Create<SmallProducerSubmissionData>();

            // Act
            var result = map.Map(source);

            // Assert
            result.ContactDetails.FirstName.Should().Be(source.CurrentSubmission.ContactData.FirstName);
            result.ContactDetails.LastName.Should().Be(source.CurrentSubmission.ContactData.LastName);
            result.ContactDetails.Position.Should().Be(source.CurrentSubmission.ContactData.Position);
        }

        [Fact]
        public void Map_ShouldMapAddressData()
        {
            // Arrange
            var source = fixture.Create<SmallProducerSubmissionData>();
            var expectedAddress = fixture.Create<AddressPostcodeRequiredData>();
            A.CallTo(() => mapper.Map<AddressData, AddressPostcodeRequiredData>(A<AddressData>._))
                .Returns(expectedAddress);

            // Act
            var result = map.Map(source);

            // Assert
            result.ContactDetails.AddressData.Should().Be(expectedAddress);
            A.CallTo(() => mapper.Map<AddressData, AddressPostcodeRequiredData>(source.CurrentSubmission.ContactAddressData))
                .MustHaveHappenedOnceExactly();
        }
    }
}