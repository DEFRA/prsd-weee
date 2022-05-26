namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.ViewModels
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using EA.Weee.Web.Areas.Aatf.Attributes;
    using EA.Weee.Web.Areas.Aatf.ViewModels;
    using FluentAssertions;
    using Xunit;

    public class SubmittedDatesFilterViewModelTests
    {
        [Theory]
        [InlineData("StartDate", "Submitted date")]
        [InlineData("EndDate", "Submitted date")]
        public void SubmittedDatesFilterViewModel_Properties_ShouldHaveDisplayNameAttribute(string property, string description)
        {
            typeof(SubmittedDatesFilterViewModel)
                .GetProperty(property)
                .Should()
                .BeDecoratedWith<DisplayNameAttribute>(d => d.DisplayName.Equals(description));
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
            typeof(EditEvidenceNoteViewModel).GetProperty("StartDate").Should().BeDecoratedWith<EvidenceNoteStartDateAttribute>();
        }

        [Fact]
        public void SubmittedDatesFilterViewModel_EndDate_ShouldHaveStartDateAttribute()
        {
            typeof(EditEvidenceNoteViewModel).GetProperty("EndDate").Should().BeDecoratedWith<EvidenceNoteEndDateAttribute>();
        }
    }
}
