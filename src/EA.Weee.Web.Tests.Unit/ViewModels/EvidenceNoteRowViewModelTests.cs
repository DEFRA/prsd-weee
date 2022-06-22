namespace EA.Weee.Web.Tests.Unit.ViewModels
{
    using System;
    using Core.AatfEvidence;
    using Core.Helpers;
    using Core.Tests.Unit.Helpers;
    using EA.Weee.Core.Shared;
    using FluentAssertions;
    using Web.Infrastructure;
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

        [Fact]
        public void AatfViewRouteName_GivenDraftNoteStatus_DraftViewShouldBeReturned()
        {
            //arrange
            model.Status = NoteStatus.Draft;

            //act
            var result = model.AatfViewRouteName;

            //assert
            result.Should().Be(AatfEvidenceRedirect.ViewDraftEvidenceRouteName);
        }

        [Fact]
        public void AatfViewRouteName_GivenSubmittedNoteStatus_SubmittedViewShouldBeReturned()
        {
            //arrange
            model.Status = NoteStatus.Submitted;

            //act
            var result = model.AatfViewRouteName;

            //assert
            result.Should().Be(AatfEvidenceRedirect.ViewSubmittedEvidenceRouteName);
        }

        [Fact]
        public void AatfViewRouteName_GivenApprovedNoteStatus_ApprovedViewShouldBeReturned()
        {
            //arrange
            model.Status = NoteStatus.Approved;

            //act
            var result = model.AatfViewRouteName;

            //assert
            result.Should().Be(AatfEvidenceRedirect.ViewApprovedEvidenceRouteName);
        }

        [Fact]
        public void AatfViewRouteName_GivenReturnedNoteStatus_ReturnedViewShouldBeReturned()
        {
            //arrange
            model.Status = NoteStatus.Returned;

            //act
            var result = model.AatfViewRouteName;

            //assert
            result.Should().Be(AatfEvidenceRedirect.ViewReturnedEvidenceRouteName);
        }

        [Fact]
        public void AatfViewRouteName_GivenRejectedNoteStatus_RejectedViewShouldBeReturned()
        {
            //arrange
            model.Status = NoteStatus.Rejected;

            //act
            var result = model.AatfViewRouteName;

            //assert
            result.Should().Be(AatfEvidenceRedirect.ViewRejectedEvidenceRouteName);
        }

        [Fact]
        public void AatfViewRouteName_GivenStatusHasNoRoute_InvalidOperationExceptionExpected()
        {
            //arrange
            model.Status = NoteStatus.Void;

            //act
            var exception = Record.Exception(() => model.AatfViewRouteName);

            //assert
            exception.Should().BeOfType<InvalidOperationException>();
        }
    }
}
