namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Mapping
{
    using System;
    using System.Linq;
    using AutoFixture;
    using Core.AatfEvidence;
    using FluentAssertions;
    using Web.Areas.Scheme.Mappings.ToViewModels;
    using Weee.Requests.Scheme;
    using Xunit;

    public class TransferEvidenceNotesViewModelMapTransferTests
    {
        private readonly Fixture fixture;

        public TransferEvidenceNotesViewModelMapTransferTests()
        {
            fixture = new Fixture();
        }

        [Fact]
        public void TransferEvidenceNotesViewModelMapTransfer_GivenNullNotes_ShouldThrowArgumentNullException()
        {
            //act
            var exception = Record.Exception(() =>
                new TransferEvidenceNotesViewModelMapTransfer(null, fixture.Create<TransferEvidenceNoteRequest>(),
                    fixture.Create<Guid>()));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void TransferEvidenceNotesViewModelMapTransfer_GivenRequest_ShouldThrowArgumentNullException()
        {
            //act
            var exception = Record.Exception(() =>
                new TransferEvidenceNotesViewModelMapTransfer(fixture.CreateMany<EvidenceNoteData>().ToList(), (TransferEvidenceNoteRequest)null,
                    fixture.Create<Guid>()));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void TransferEvidenceNotesViewModelMapTransfer_GivenEmptyOrganisationId_ShouldThrowArgumentException()
        {
            //act
            var exception = Record.Exception(() =>
                new TransferEvidenceNotesViewModelMapTransfer(fixture.CreateMany<EvidenceNoteData>().ToList(), fixture.Create<TransferEvidenceNoteRequest>(), Guid.Empty));

            //assert
            exception.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void TransferEvidenceNotesViewModelMapTransfer_GivenValues_PropertiesShouldBeSet()
        {
            //arrange
            var notes = fixture.CreateMany<EvidenceNoteData>().ToList();
            var request = fixture.Create<TransferEvidenceNoteRequest>();
            var organisationId = fixture.Create<Guid>();

            //act
            var result = new TransferEvidenceNotesViewModelMapTransfer(notes, request, organisationId);

            //assert
            result.OrganisationId.Should().Be(organisationId);
            result.Notes.ToList().Should().BeEquivalentTo(notes);
            result.Request.Should().Be(request);
        }
    }
}
