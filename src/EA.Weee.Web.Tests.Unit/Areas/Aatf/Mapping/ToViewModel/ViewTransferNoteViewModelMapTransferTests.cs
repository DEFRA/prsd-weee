namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Mapping.ToViewModel
{
    using System;
    using AutoFixture;
    using Core.AatfEvidence;
    using FluentAssertions;
    using Web.Areas.Scheme.Mappings.ToViewModels;
    using Xunit;

    public class ViewTransferNoteViewModelMapTransferTests
    {
        private readonly Fixture fixture;

        public ViewTransferNoteViewModelMapTransferTests()
        {
            fixture = new Fixture();
        }

        [Fact]
        public void ViewTransferNoteViewModelMapTransfer_GivenEmptySchemeId_ArgumentExceptionExpected()
        {
            //act
            var exception = Record.Exception(() =>
                new ViewTransferNoteViewModelMapTransfer(Guid.Empty, new TransferEvidenceNoteData(), null));

            //assert
            exception.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void ViewTransferNoteViewModelMapTransfer_GivenEmptySchemeIdAndOtherPropertiesAreValid_ArgumentExceptionExpected()
        {
            //act
            var exception = Record.Exception(() =>
                new ViewTransferNoteViewModelMapTransfer(Guid.Empty, new TransferEvidenceNoteData(), new object()));

            //assert
            exception.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void ViewTransferNoteViewModelMapTransfer_GivenNullTransferNoteData_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() =>
                new ViewTransferNoteViewModelMapTransfer(fixture.Create<Guid>(), null, null));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void ViewTransferNoteViewModelMapTransfer_GivenValues_PropertiesShouldBeSet()
        {
            //arrange
            var schemeId = fixture.Create<Guid>();
            var note = fixture.Create<TransferEvidenceNoteData>();
            var status = note.Status;

            //act
            var result = new ViewTransferNoteViewModelMapTransfer(schemeId, note, null);

            //assert
            result.TransferEvidenceNoteData.Should().Be(note);
            result.SchemeId.Should().Be(schemeId);
            result.TransferEvidenceNoteData.Status.Should().Be(status);
        }

        [Fact]
        public void ViewTransferNoteViewModelMapTransfer_GivenValuesWithNullStatus_PropertiesShouldBeSet()
        {
            //arrange
            var schemeId = fixture.Create<Guid>();
            var note = fixture.Create<TransferEvidenceNoteData>();

            //act
            var result = new ViewTransferNoteViewModelMapTransfer(schemeId, note, null);

            //assert
            result.TransferEvidenceNoteData.Should().Be(note);
            result.SchemeId.Should().Be(schemeId);
            result.DisplayNotification.Should().BeNull();
        }
    }
}
