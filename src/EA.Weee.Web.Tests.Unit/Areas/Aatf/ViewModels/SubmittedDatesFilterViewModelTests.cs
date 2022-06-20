namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.ViewModels
{
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
            typeof(EditEvidenceNoteViewModel).GetProperty("StartDate")
                .Should().BeDecoratedWith<EvidenceNoteStartDateAttribute>(e => e.CheckComplianceYear == false
                && e.CompareDatePropertyName.Equals("EndDate"));
        }

        [Fact]
        public void SubmittedDatesFilterViewModel_EndDate_ShouldHaveStartDateAttribute()
        {
            typeof(EditEvidenceNoteViewModel)
                .GetProperty("EndDate").Should().BeDecoratedWith<EvidenceNoteEndDateAttribute>(e => e.CompareDatePropertyName.Equals("StartDate"));
        }
    }
}
