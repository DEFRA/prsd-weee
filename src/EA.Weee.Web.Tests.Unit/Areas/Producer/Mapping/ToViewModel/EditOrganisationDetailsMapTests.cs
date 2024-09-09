namespace EA.Weee.Web.Tests.Unit.Areas.Producer.Mapping.ToViewModel
{
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Web.Areas.Producer.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.Producer.ViewModels;
    using FluentAssertions;
    using Xunit;

    public class EditOrganisationDetailsMapTests
    {
        private readonly EditOrganisationDetailsMap mapper;

        public EditOrganisationDetailsMapTests()
        {
            mapper = new EditOrganisationDetailsMap();
        }

        [Fact]
        public void Map_ReturnsNewEditOrganisationDetailsViewModel()
        {
            // Arrange
            var source = new SmallProducerSubmissionData();

            // Act
            var result = mapper.Map(source);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<EditOrganisationDetailsViewModel>();
        }

        [Fact]
        public void Map_WithNullSource_ReturnsNewEditOrganisationDetailsViewModel()
        {
            // Arrange
            SmallProducerSubmissionData source = null;

            // Act
            var result = mapper.Map(source);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<EditOrganisationDetailsViewModel>();
        }
    }
}