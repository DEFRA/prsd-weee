namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Mapping.ToViewModel
{
    using System;
    using AutoFixture;
    using EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.Aatf.ViewModels;
    using FluentAssertions;
    using Xunit;

    public class SubmittedDatesFilterViewModelMapTests
    {
        private readonly SubmittedDatesFilterViewModelMap mapper;

        private Fixture fixture;

        public SubmittedDatesFilterViewModelMapTests()
        {
            fixture = new Fixture();

            mapper = new SubmittedDatesFilterViewModelMap();
        }

        [Fact]
        public void Map_GivenNullSource_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => mapper.Map(null));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenSource_PropertiesShouldBeSet()
        {
            //arrange
            var source = fixture.Create<SubmittedDateFilterBase>();

            //act
            var model = mapper.Map(source);

            // assert
            model.StartDate.Should().Be(source.StartDate);
            model.EndDate.Should().Be(source.EndDate);
        }
    }
}
