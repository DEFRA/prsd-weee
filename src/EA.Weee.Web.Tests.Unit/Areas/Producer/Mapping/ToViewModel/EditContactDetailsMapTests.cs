﻿namespace EA.Weee.Web.Tests.Unit.Areas.Producer.Mapping.ToViewModel
{
    using AutoFixture;
    using AutoFixture.AutoFakeItEasy;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Organisations;
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
        public void Map_ShouldMapHighLevelSourceFields()
        {
            // Arrange
            var source = fixture.Create<SmallProducerSubmissionMapperData>();
            var submissionData = source.SmallProducerSubmissionData;

            // Act
            var result = map.Map(source);

            // Assert
            result.DirectRegistrantId.Should().Be(submissionData.DirectRegistrantId);
            result.OrganisationId.Should().Be(submissionData.OrganisationData.Id);
            result.HasAuthorisedRepresentitive.Should().Be(submissionData.HasAuthorisedRepresentitive);
            result.RedirectToCheckAnswers.Should().Be(source.RedirectToCheckAnswers);
        }

        [Fact]
        public void Map_ShouldUseCurrentSubmissionDataWhenAvailable()
        {
            // Arrange
            var source = fixture.Create<SmallProducerSubmissionMapperData>();
            var submissionData = source.SmallProducerSubmissionData;

            // Act
            var result = map.Map(source);

            // Assert
            result.ContactDetails.FirstName.Should().Be(submissionData.CurrentSubmission.ContactData.FirstName);
            result.ContactDetails.LastName.Should().Be(submissionData.CurrentSubmission.ContactData.LastName);
            result.ContactDetails.Position.Should().Be(submissionData.CurrentSubmission.ContactData.Position);
        }

        [Fact]
        public void Map_ShouldMapAddressData()
        {
            // Arrange
            var source = fixture.Create<SmallProducerSubmissionMapperData>();
            var submissionData = source.SmallProducerSubmissionData;
            var expectedAddress = fixture.Create<AddressPostcodeRequiredData>();
            A.CallTo(() => mapper.Map<AddressData, AddressPostcodeRequiredData>(A<AddressData>._))
                .Returns(expectedAddress);

            // Act
            var result = map.Map(source);

            // Assert
            result.ContactDetails.AddressData.Should().Be(expectedAddress);
            A.CallTo(() => mapper.Map<AddressData, AddressPostcodeRequiredData>(submissionData.CurrentSubmission.ContactAddressData))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Map_ShouldHandleNullContactData()
        {
            // Arrange
            var source = fixture.Create<SmallProducerSubmissionMapperData>();
            source.SmallProducerSubmissionData.CurrentSubmission.ContactData = null;

            // Act
            var result = map.Map(source);

            // Assert
            result.ContactDetails.Should().NotBeNull();
            result.ContactDetails.FirstName.Should().BeNull();
            result.ContactDetails.LastName.Should().BeNull();
            result.ContactDetails.Position.Should().BeNull();
            result.ContactDetails.AddressData.Should().NotBeNull();
        }

        [Fact]
        public void Map_ShouldAlwaysInitializeAddressData()
        {
            // Arrange
            var source = fixture.Create<SmallProducerSubmissionMapperData>();
            source.SmallProducerSubmissionData.CurrentSubmission.ContactData = null;

            // Act
            var result = map.Map(source);

            // Assert
            result.ContactDetails.AddressData.Should().NotBeNull();
            result.ContactDetails.AddressData.Should().BeOfType<AddressPostcodeRequiredData>();
        }
    }
}