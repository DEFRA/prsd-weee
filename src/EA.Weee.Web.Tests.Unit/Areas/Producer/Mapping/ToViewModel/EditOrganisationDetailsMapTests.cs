namespace EA.Weee.Web.Tests.Unit.Areas.Producer.Mapping.ToViewModel
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Web.Areas.Producer.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.Producer.ViewModels;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class EditOrganisationDetailsMapTests
    {
        private readonly EditOrganisationDetailsMap map;
        private readonly IMapper mapper;

        public EditOrganisationDetailsMapTests()
        {
            mapper = A.Fake<IMapper>();

            map = new EditOrganisationDetailsMap(mapper);
        }

        [Fact]
        public void Map_ReturnsNewEditOrganisationDetailsViewModel()
        {
            // Arrange
            var source = new SmallProducerSubmissionData();

            // Act
            var result = map.Map(source);

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
            var result = map.Map(source);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<EditOrganisationDetailsViewModel>();
        }
    }
}