namespace EA.Weee.Web.Tests.Unit.ViewModels
{
    using System;
    using Core.AatfEvidence;
    using Core.Helpers;
    using FluentAssertions;
    using Web.Infrastructure;
    using Web.ViewModels.Shared;
    using Weee.Tests.Core.DataHelpers;
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
        public void AatfViewRouteName_GivenVoidNoteStatus_VoidedViewShouldBeReturned()
        {
            //arrange
            model.Status = NoteStatus.Void;

            //act
            var result = model.AatfViewRouteName;

            //assert
            result.Should().Be(AatfEvidenceRedirect.ViewVoidedEvidenceRouteName);
        }

        [Fact]
        public void SchemeViewRouteName_GivenTransferNoteAndDraftStatus_DraftRouteShouldBeReturned()
        {
            //arrange
            model.Status = NoteStatus.Draft;
            model.Type = NoteType.Transfer;

            //act
            var result = model.SchemeViewRouteName;

            //assert
            result.Should().Be(SchemeTransferEvidenceRedirect.ViewDraftTransferEvidenceRouteName);
        }

        [Fact]
        public void SchemeViewRouteName_GivenTransferNoteAndSubmittedStatus_SubmittedRouteShouldBeReturned()
        {
            //arrange
            model.Status = NoteStatus.Submitted;
            model.Type = NoteType.Transfer;

            //act
            var result = model.SchemeViewRouteName;

            //assert
            result.Should().Be(SchemeTransferEvidenceRedirect.ViewSubmittedTransferEvidenceRouteName);
        }

        [Fact]
        public void SchemeViewRouteName_GivenTransferNoteAndApprovedStatus_ApprovedRouteShouldBeReturned()
        {
            //arrange
            model.Status = NoteStatus.Approved;
            model.Type = NoteType.Transfer;

            //act
            var result = model.SchemeViewRouteName;

            //assert
            result.Should().Be(SchemeTransferEvidenceRedirect.ViewApprovedTransferEvidenceRouteName);
        }

        [Fact]
        public void SchemeViewRouteName_GivenTransferNoteAndReturnedStatus_ReturnedRouteShouldBeReturned()
        {
            //arrange
            model.Status = NoteStatus.Returned;
            model.Type = NoteType.Transfer;

            //act
            var result = model.SchemeViewRouteName;

            //assert
            result.Should().Be(SchemeTransferEvidenceRedirect.ViewReturnedTransferEvidenceRouteName);
        }

        [Fact]
        public void SchemeViewRouteName_GivenTransferNoteAndRejectedStatus_RejectedRouteShouldBeReturned()
        {
            //arrange
            model.Status = NoteStatus.Rejected;
            model.Type = NoteType.Transfer;

            //act
            var result = model.SchemeViewRouteName;

            //assert
            result.Should().Be(SchemeTransferEvidenceRedirect.ViewRejectedTransferEvidenceRouteName);
        }

        [Fact]
        public void SchemeViewRouteName_GivenTransferNoteAndVoidNoteStatus_VoidedViewShouldBeReturned()
        {
            //arrange
            model.Status = NoteStatus.Void;
            model.Type = NoteType.Transfer;

            //act
            var route = model.SchemeViewRouteName;

            //assert
            route.Should().Be(SchemeTransferEvidenceRedirect.ViewVoidedTransferEvidenceRouteName);
        }

        [Fact]
        public void SchemeViewRouteName_GivenEvidenceNoteAndVoidNoteStatus_VoidedViewShouldBeReturned()
        {
            //arrange
            model.Status = NoteStatus.Void;
            model.Type = NoteType.Evidence;

            //act
            var route = model.SchemeViewRouteName;

            //assert
            route.Should().Be(SchemeTransferEvidenceRedirect.ViewVoidedEvidenceNoteRouteName);
        }

        [Fact]
        public void SchemeViewRouteName_GivenEvidenceNoteAndApprovedNoteStatus_ApprovedViewShouldBeReturned()
        {
            //arrange
            model.Status = NoteStatus.Approved;
            model.Type = NoteType.Evidence;

            //act
            var route = model.SchemeViewRouteName;

            //assert
            route.Should().Be(SchemeTransferEvidenceRedirect.ViewApprovedEvidenceNoteRouteName);
        }

        [Fact]
        public void SchemeViewRouteName_GivenEvidenceNoteAndSubmittedNoteStatus_SubmittedViewRouteShouldBeReturned()
        {
            //arrange
            model.Status = NoteStatus.Submitted;
            model.Type = NoteType.Evidence;

            //act
            var route = model.SchemeViewRouteName;

            //assert
            route.Should().Be(SchemeTransferEvidenceRedirect.ViewSubmittedEvidenceNoteRouteName);
        }

        [Fact]
        public void SchemeViewRouteName_GivenEvidenceNoteAndDraftNoteStatus_EmptyRouteShouldBeReturned()
        {
            //arrange
            model.Status = NoteStatus.Draft;
            model.Type = NoteType.Evidence;

            //act
            var route = model.SchemeViewRouteName;

            //assert
            route.Should().BeEmpty();
        }

        [Fact]
        public void SchemeViewRouteName_GivenEvidenceNoteAndRejectedNoteStatus_RejectedViewRouteShouldBeReturned()
        {
            //arrange
            model.Status = NoteStatus.Rejected;
            model.Type = NoteType.Evidence;

            //act
            var route = model.SchemeViewRouteName;

            //assert
            route.Should().Be(SchemeTransferEvidenceRedirect.ViewRejectedEvidenceNoteRouteName);
        }

        [Fact]
        public void SchemeViewRouteName_GivenEvidenceNoteAndReturnedNoteStatus_ReturnedViewRouteShouldBeReturned()
        {
            //arrange
            model.Status = NoteStatus.Returned;
            model.Type = NoteType.Evidence;

            //act
            var route = model.SchemeViewRouteName;

            //assert
            route.Should().Be(SchemeTransferEvidenceRedirect.ViewReturnedEvidenceNoteRouteName);
        }
    }
}
