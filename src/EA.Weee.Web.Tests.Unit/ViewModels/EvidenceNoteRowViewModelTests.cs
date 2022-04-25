namespace EA.Weee.Web.Tests.Unit.ViewModels
{
    using System;
    using Core.AatfEvidence;
    using Core.Helpers;
    using Core.Tests.Unit.Helpers;
    using FluentAssertions;
    using Web.ViewModels.Shared;
    using Xunit;

    public class EvidenceNoteRowViewModelTests
    {
        private readonly EvidenceNoteRowViewModel model;

        public EvidenceNoteRowViewModelTests()
        {
            model = new EvidenceNoteRowViewModel();
        }

        [Fact]
        public void SubmittedDateDisplay_GivenNoSubmittedDate_ShouldBeFormattedCorrectly()
        {
            //arrange
            model.SubmittedDate = null;

            //act
            var result = model.SubmittedDateDisplay;

            //assert
            result.Should().Be("-");
        }

        [Fact]
        public void SubmittedDateDisplay_GivenSubmittedDate_ShouldBeFormattedCorrectly()
        {
            //arrange
            var date = new DateTime();
            model.SubmittedDate = date;

            //act
            var result = model.SubmittedDateDisplay;

            //assert
            result.Should().Be(date.ToShortDateString());
        }

        [Theory]
        [ClassData(typeof(NoteTypeCoreData))]
        public void ReferenceDisplay_GivenReference_ShouldBeFormattedCorrectly(NoteType type)
        {
            //arrange
            model.ReferenceId = 1;
            model.Type = type;

            //act
            var result = model.ReferenceDisplay;

            //assert
            result.Should().Be($"{type.ToDisplayString()}1");
        }
    }
}
