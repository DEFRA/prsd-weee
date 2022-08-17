namespace EA.Weee.Web.Tests.Unit.Areas.Admin.ViewModels
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using FluentAssertions;
    using Web.Areas.Admin.ViewModels.ManageEvidenceNotes;
    using Xunit;

    public class VoidNoteViewModelBaseTests
    {
        [Fact]
        public void VoidNoteViewModelBase_ShouldBeAbstract()
        {
            typeof(VoidNoteViewModelBase).Should().BeAbstract();
        }

        [Fact]
        public void VoidedReason_ShouldHaveRequiredAttribute()
        {
            typeof(VoidNoteViewModelBase).GetProperty("VoidedReason").Should().BeDecoratedWith<RequiredAttribute>()
                .Which.ErrorMessage.Should().Be("Enter a reason for voiding the note");
        }

        [Fact]
        public void VoidedReason_ShouldHaveDisplayAttribute()
        {
            typeof(VoidNoteViewModelBase).GetProperty("VoidedReason").Should().BeDecoratedWith<DisplayNameAttribute>()
                .Which.DisplayName.Should().Be("Reason");
        }

        [Fact]
        public void VoidedReason_ShouldHaveStringLengthAttribute()
        {
            typeof(VoidNoteViewModelBase).GetProperty("VoidedReason").Should().BeDecoratedWith<StringLengthAttribute>()
                .Which.MaximumLength.Should().Be(200);
        }
    }
}
