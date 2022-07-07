namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Mapping
{
    using System;
    using System.Linq;
    using AutoFixture;
    using Core.AatfEvidence;
    using Core.Scheme;
    using FluentAssertions;
    using Web.Areas.Scheme.Mappings.ToViewModels;
    using Web.Areas.Scheme.ViewModels;
    using Weee.Requests.Scheme;
    using Weee.Tests.Core;
    using Xunit;

    public class TransferEvidenceNotesViewModelMapTransferTests : SimpleUnitTestBase
    {
        [Fact]
        public void TransferEvidenceNotesViewModelMapTransfer_GivenNullNotes_ShouldThrowArgumentNullException()
        {
            //act
            var exception = Record.Exception(() =>
                new TransferEvidenceNotesViewModelMapTransfer(null, TestFixture.Create<TransferEvidenceNoteRequest>(),
                    TestFixture.Create<Guid>()));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void TransferEvidenceNotesViewModelMapTransfer_GivenNullRequest_ShouldThrowArgumentNullException()
        {
            //act
            var exception = Record.Exception(() =>
                new TransferEvidenceNotesViewModelMapTransfer(TestFixture.CreateMany<EvidenceNoteData>().ToList(), 
                    null,
                    TestFixture.Create<TransferEvidenceNoteData>(),
                    TestFixture.Create<Guid>()));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void TransferEvidenceNotesViewModelMapTransfer_GivenNullNotesWithTransferNoteData_ShouldThrowArgumentNullException()
        {
            //act
            var exception = Record.Exception(() =>
                new TransferEvidenceNotesViewModelMapTransfer(null, 
                    TestFixture.Create<TransferEvidenceNoteRequest>(), 
                    TestFixture.Create<TransferEvidenceNoteData>(), 
                    TestFixture.Create<Guid>()));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void TransferEvidenceNotesViewModelMapTransfer_GivenTransferNotesNullRequest_ShouldThrowArgumentNullException()
        {
            //act
            var exception = Record.Exception(() =>
                new TransferEvidenceNotesViewModelMapTransfer(TestFixture.CreateMany<EvidenceNoteData>().ToList(), 
                    null,
                    TestFixture.Create<Guid>()));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void TransferEvidenceNotesViewModelMapTransfer_GivenNullTransferNoteData_ShouldThrowArgumentNullException()
        {
            //act
            var exception = Record.Exception(() =>
                new TransferEvidenceNotesViewModelMapTransfer(TestFixture.CreateMany<EvidenceNoteData>().ToList(),
                    TestFixture.Create<TransferEvidenceNoteRequest>(),
                    null,
                    TestFixture.Create<Guid>()));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void TransferEvidenceNotesViewModelMapTransfer_GivenEmptyOrganisationIdWithTransferNoteData_ShouldThrowArgumentException()
        {
            //act
            var exception = Record.Exception(() =>
                new TransferEvidenceNotesViewModelMapTransfer(TestFixture.CreateMany<EvidenceNoteData>().ToList(),
                    TestFixture.Create<TransferEvidenceNoteRequest>(),
                    TestFixture.Create<TransferEvidenceNoteData>(), 
                    Guid.Empty));

            //assert
            exception.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void TransferEvidenceNotesViewModelMapTransfer_GivenEmptyOrganisationId_ShouldThrowArgumentException()
        {
            //act
            var exception = Record.Exception(() =>
                new TransferEvidenceNotesViewModelMapTransfer(TestFixture.CreateMany<EvidenceNoteData>().ToList(), 
                    TestFixture.Create<TransferEvidenceNoteRequest>(), 
                    Guid.Empty));

            //assert
            exception.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void TransferEvidenceNotesViewModelMapTransfer_GivenRequestValues_PropertiesShouldBeSet()
        {
            //arrange
            var notes = TestFixture.CreateMany<EvidenceNoteData>().ToList();
            var request = TestFixture.Create<TransferEvidenceNoteRequest>();
            var organisationId = TestFixture.Create<Guid>();

            //act
            var result = new TransferEvidenceNotesViewModelMapTransfer(notes, request, organisationId);

            //assert
            result.OrganisationId.Should().Be(organisationId);
            result.Notes.ToList().Should().BeEquivalentTo(notes);
            result.Request.Should().Be(request);
        }

        [Fact]
        public void TransferEvidenceNotesViewModelMapTransfer_GivenTransferNoteDataValues_PropertiesShouldBeSet()
        {
            //arrange
            var notes = TestFixture.CreateMany<EvidenceNoteData>().ToList();
            var noteData = TestFixture.Create<TransferEvidenceNoteData>();
            var organisationId = TestFixture.Create<Guid>();
            var request = TestFixture.Create<TransferEvidenceNoteRequest>();

            //act
            var result = new TransferEvidenceNotesViewModelMapTransfer(notes, request, noteData, organisationId);

            //assert
            result.OrganisationId.Should().Be(organisationId);
            result.Notes.ToList().Should().BeEquivalentTo(notes);
            result.TransferEvidenceNoteData.Should().Be(noteData);
        }

        [Fact]
        public void TransferEvidenceNotesViewModelMapTransfer_GivenTransferCategoryConstructorValues_PropertiesShouldBeSet()
        {
            //arrange
            var schemeData = TestFixture.CreateMany<SchemeData>().ToList();
            var noteData = TestFixture.Create<TransferEvidenceNoteData>();
            var organisationId = TestFixture.Create<Guid>();

            //act
            var result = new TransferEvidenceNotesViewModelMapTransfer(noteData, schemeData, organisationId, null);

            //assert
            result.OrganisationId.Should().Be(organisationId);
            result.SchemeData.Should().BeEquivalentTo(schemeData);
            result.TransferEvidenceNoteData.Should().Be(noteData);
            result.ExistingTransferEvidenceNoteCategoriesViewModel.Should().BeNull();
        }

        [Fact]
        public void TransferEvidenceNotesViewModelMapTransfer_GivenTransferCategoryConstructorValuesWithExistingModel_PropertiesShouldBeSet()
        {
            //arrange
            var schemeData = TestFixture.CreateMany<SchemeData>().ToList();
            var noteData = TestFixture.Create<TransferEvidenceNoteData>();
            var organisationId = TestFixture.Create<Guid>();
            var model = TestFixture.Create<TransferEvidenceNoteCategoriesViewModel>();

            //act
            var result = new TransferEvidenceNotesViewModelMapTransfer(noteData, schemeData, organisationId, model);

            //assert
            result.OrganisationId.Should().Be(organisationId);
            result.SchemeData.Should().BeEquivalentTo(schemeData);
            result.TransferEvidenceNoteData.Should().Be(noteData);
            result.ExistingTransferEvidenceNoteCategoriesViewModel.Should().Be(model);
        }

        [Fact]
        public void TransferEvidenceNotesViewModelMapTransfer_GivenCategoryConstructorAndTransferNoteDataIsNull_ArgumentNullExceptionExpected()
        {
            //arrange
            var schemeData = TestFixture.CreateMany<SchemeData>().ToList();
            var organisationId = TestFixture.Create<Guid>();

            //act
            var exception = Record.Exception(() => new TransferEvidenceNotesViewModelMapTransfer(null, schemeData, organisationId, null));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void TransferEvidenceNotesViewModelMapTransfer_GivenCategoryConstructorAndSchemeDataIsNull_ArgumentNullExceptionExpected()
        {
            //arrange
            var noteData = TestFixture.Create<TransferEvidenceNoteData>();
            var organisationId = TestFixture.Create<Guid>();

            //act
            var exception = Record.Exception(() => new TransferEvidenceNotesViewModelMapTransfer(noteData, null, organisationId, null));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void TransferEvidenceNotesViewModelMapTransfer_GivenCategoryConstructorAndOrganisationIdIsEmpty_ArgumentExceptionExpected()
        {
            //arrange
            var schemeData = TestFixture.CreateMany<SchemeData>().ToList();
            var noteData = TestFixture.Create<TransferEvidenceNoteData>();

            //act
            var exception = Record.Exception(() => new TransferEvidenceNotesViewModelMapTransfer(noteData, schemeData, Guid.Empty, null));

            //assert
            exception.Should().BeOfType<ArgumentException>();
        }
    }
}
