namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.ViewModels
{
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Core.Constants;
    using EA.Weee.Web.Areas.Scheme.ViewModels;
    using FluentAssertions;
    using System;
    using System.ComponentModel;
    using Xunit;

    public class EvidenceNoteHistoryViewModelTests
    {
        [Fact]
        public void ReferenceDisplay_ShouldHave_DisplayAttribute()
        {
            //assert
            typeof(EvidenceNoteHistoryViewModel).GetProperty("ReferenceDisplay")
            .Should()
            .BeDecoratedWith<DisplayNameAttribute>(a => a.DisplayName.Equals("Reference ID"));
        }

        [Theory]
        [InlineData(NoteType.Evidence, "E1")]
        [InlineData(NoteType.Transfer, "T1")]
        public void ReferenceDisplay_ShouldDisplayCorrectType(NoteType type, string expectedDisplay)
        {
            //act
            var model = new EvidenceNoteHistoryViewModel(new Guid(), 1, string.Empty, type, NoteStatus.Approved, null);

            //assert
            model.ReferenceDisplay.Should().Be($"{expectedDisplay}");
        }

        [Fact]
        public void PopulatedSubmittedDateDisplay_ShouldDisplay_CorrectGMTFormat()
        {
            //arrange
            var date = DateTime.Parse("01/01/2022");
            
            //act
            var model = new EvidenceNoteHistoryViewModel(new Guid(), 1, string.Empty, NoteType.Transfer, NoteStatus.Approved, date);
            
            //assert
            model.SubmittedDateDisplay.Should().Be(date.ToString(DateTimeConstants.GeneralDateTimeGMTFormat));
        }

        [Fact]
        public void NonPopulatedSubmittedDateDisplay_ShouldDisplay_Dash()
        {
            //act
            var model = new EvidenceNoteHistoryViewModel(new Guid(), 1, string.Empty, NoteType.Transfer, NoteStatus.Approved, null);

            //assert
            model.SubmittedDateDisplay.Should().Be("-");
        }
    }
}
