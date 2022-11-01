namespace EA.Weee.Web.Tests.Unit.ViewModels
{
    using AutoFixture;
    using EA.Weee.Tests.Core;
    using EA.Weee.Web.Areas.Aatf.Attributes;
    using EA.Weee.Web.ViewModels.Shared;
    using FluentAssertions;
    using System;
    using System.ComponentModel.DataAnnotations;
    using Xunit;

    public class SubmittedDatesFilterViewModelTests : SimpleUnitTestBase
    {
        [Theory]
        [InlineData("StartDate", "Submitted start date")]
        [InlineData("EndDate", "Submitted end date")]
        public void SubmittedDatesFilterViewModel_Properties_ShouldHaveDisplayNameAttribute(string property, string description)
        {
            typeof(SubmittedDatesFilterViewModel)
                .GetProperty(property)
                .Should()
                .BeDecoratedWith<DisplayAttribute>(d => d.Name.Equals(description));
        }

        [Theory]
        [InlineData("StartDate", DataType.Date)]
        [InlineData("EndDate", DataType.Date)]
        public void SubmittedDatesFilterViewModel_Properties_ShouldHaveDataTypeAttribute(string property, DataType type)
        {
            typeof(SubmittedDatesFilterViewModel).GetProperty(property).Should().BeDecoratedWith<DataTypeAttribute>(d => d.DataType.Equals(type));
        }

        [Fact]
        public void SubmittedDatesFilterViewModel_StartDate_ShouldHaveStartDateAttribute()
        {
            typeof(SubmittedDatesFilterViewModel).GetProperty("StartDate")
                .Should().BeDecoratedWith<EvidenceNoteFilterStartDateAttribute>(e => e.CompareDatePropertyName.Equals("EndDate"));
        }

        [Fact]
        public void SubmittedDatesFilterViewModel_EndDate_ShouldHaveStartDateAttribute()
        {
            typeof(SubmittedDatesFilterViewModel)
                .GetProperty("EndDate").Should().BeDecoratedWith<EvidenceNoteFilterEndDateAttribute>(e => e.CompareDatePropertyName.Equals("StartDate"));
        }

        [Fact]
        public void SearchPerformed_GivenNoSearchPerformed_FalseShouldBeReturned()
        {
            //arrange
            var model = new SubmittedDatesFilterViewModel();

            //act
            var searchPerformed = model.SearchPerformed;

            //assert
            searchPerformed.Should().BeFalse();
        }

        [Fact]
        public void SearchPerformed_GivenStartDateSelected_TrueShouldBeReturned()
        {
            //arrange
            var model = new SubmittedDatesFilterViewModel()
            {
                StartDate = TestFixture.Create<DateTime>()
            };

            //act
            var searchPerformed = model.SearchPerformed;

            //assert
            searchPerformed.Should().BeTrue();
        }

        [Fact]
        public void SearchPerformed_GivenEndDateSelected_TrueShouldBeReturned()
        {
            //arrange
            var model = new SubmittedDatesFilterViewModel()
            {
                EndDate = TestFixture.Create<DateTime>()
            };

            //act
            var searchPerformed = model.SearchPerformed;

            //assert
            searchPerformed.Should().BeTrue();
        }
    }
}
