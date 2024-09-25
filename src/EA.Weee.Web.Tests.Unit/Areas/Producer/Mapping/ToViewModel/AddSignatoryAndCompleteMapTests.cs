namespace EA.Weee.Web.Tests.Unit.Areas.Producer.Mapping.ToViewModel
{
    using AutoFixture;
    using AutoFixture.AutoFakeItEasy;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Web.Areas.Producer.Mappings.ToViewModel;
    using FluentAssertions;
    using Xunit;

    public class AddSignatoryAndCompleteMapTests
    {
        private readonly IFixture fixture;
        private readonly IMapper mapper;
        private readonly AddSignatoryAndCompleteMap map;

        public AddSignatoryAndCompleteMapTests()
        {
            fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            mapper = fixture.Freeze<IMapper>();
            map = new AddSignatoryAndCompleteMap(mapper);
        }

        [Fact]
        public void Map_ShouldMaphighLevelSourceFields()
        {
            // Arrange
            var source = fixture.Create<SmallProducerSubmissionMapperData>();
            var submissionData = source.SmallProducerSubmissionData;

            // Act
            var result = map.Map(submissionData);

            // Assert
            result.DirectRegistrantId.Should().Be(submissionData.DirectRegistrantId);
            result.OrganisationId.Should().Be(submissionData.OrganisationData.Id);
            result.HasAuthorisedRepresentitive.Should().Be(submissionData.HasAuthorisedRepresentitive);
        }

        [Fact]
        public void Map_ShouldUseCurrentSubmissionDataWhenAvailable()
        {
            // Arrange
            var source = fixture.Create<SmallProducerSubmissionMapperData>();
            var submissionData = source.SmallProducerSubmissionData;

            // Act
            var result = map.Map(submissionData);

            // Assert
            result.Contact.FirstName.Should().Be(submissionData.CurrentSubmission.ContactData.FirstName);
            result.Contact.LastName.Should().Be(submissionData.CurrentSubmission.ContactData.LastName);
            result.Contact.Position.Should().Be(submissionData.CurrentSubmission.ContactData.Position);
        }
    }
}