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
                new TransferEvidenceNotesViewModelMapTransfer(TestFixture.Create<int>(), null, TestFixture.Create<TransferEvidenceNoteRequest>(),
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
                new TransferEvidenceNotesViewModelMapTransfer(TestFixture.Create<int>(), 
                    new EvidenceNoteSearchDataResult(TestFixture.CreateMany<EvidenceNoteData>(3).ToList(), 3), 
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
                new TransferEvidenceNotesViewModelMapTransfer(new EvidenceNoteSearchDataResult(TestFixture.CreateMany<EvidenceNoteData>(3).ToList(), 3),
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
                new TransferEvidenceNotesViewModelMapTransfer(new EvidenceNoteSearchDataResult(TestFixture.CreateMany<EvidenceNoteData>(3).ToList(), 3),
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
                new TransferEvidenceNotesViewModelMapTransfer(TestFixture.Create<int>(), 
                    new EvidenceNoteSearchDataResult(TestFixture.CreateMany<EvidenceNoteData>(3).ToList(), 3), 
                    TestFixture.Create<TransferEvidenceNoteRequest>(), 
                    Guid.Empty));

            //assert
            exception.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void TransferEvidenceNotesViewModelMapTransfer_GivenRequestValues_PropertiesShouldBeSet()
        {
            //arrange
            var notes = new EvidenceNoteSearchDataResult(TestFixture.CreateMany<EvidenceNoteData>(3).ToList(), 3);
            var request = TestFixture.Create<TransferEvidenceNoteRequest>();
            var organisationId = TestFixture.Create<Guid>();
            var complianceYear = TestFixture.Create<int>();
            //act
            var result = new TransferEvidenceNotesViewModelMapTransfer(complianceYear, notes, request, organisationId);

            //assert
            result.OrganisationId.Should().Be(organisationId);
            result.Notes.Should().BeEquivalentTo(notes);
            result.Request.Should().Be(request);
            result.ComplianceYear.Should().Be(complianceYear);
        }

        [Fact]
        public void TransferEvidenceNotesViewModelMapTransfer_GivenTransferNoteDataValues_PropertiesShouldBeSet()
        {
            //arrange
            var notes = new EvidenceNoteSearchDataResult(TestFixture.CreateMany<EvidenceNoteData>(3).ToList(), 3);
            var noteData = TestFixture.Create<TransferEvidenceNoteData>();
            var organisationId = TestFixture.Create<Guid>();
            var request = TestFixture.Create<TransferEvidenceNoteRequest>();

            //act
            var result = new TransferEvidenceNotesViewModelMapTransfer(notes, request, noteData, organisationId);

            //assert
            result.OrganisationId.Should().Be(organisationId);
            result.Notes.Should().BeEquivalentTo(notes);
            result.TransferEvidenceNoteData.Should().Be(noteData);
        }

        [Fact]
        public void TransferEvidenceNotesViewModelMapTransfer_GivenTransferCategoryConstructorValues_PropertiesShouldBeSet()
        {
            //arrange
            var recipientData = TestFixture.CreateMany<OrganisationSchemeData>().ToList();
            var noteData = TestFixture.Create<TransferEvidenceNoteData>();
            var organisationId = TestFixture.Create<Guid>();

            //act
            var result = new TransferEvidenceNotesViewModelMapTransfer(noteData, recipientData, organisationId, null);

            //assert
            result.OrganisationId.Should().Be(organisationId);
            result.RecipientData.Should().BeEquivalentTo(recipientData);
            result.TransferEvidenceNoteData.Should().Be(noteData);
            result.ExistingTransferEvidenceNoteCategoriesViewModel.Should().BeNull();
        }

        [Fact]
        public void TransferEvidenceNotesViewModelMapTransfer_GivenTransferCategoryConstructorValuesWithExistingModel_PropertiesShouldBeSet()
        {
            //arrange
            var recipientData = TestFixture.CreateMany<OrganisationSchemeData>().ToList();
            var noteData = TestFixture.Create<TransferEvidenceNoteData>();
            var organisationId = TestFixture.Create<Guid>();
            var model = TestFixture.Build<TransferEvidenceNoteCategoriesViewModel>()
                .With(t => t.CategoryBooleanViewModels, new CategoryValues<CategoryBooleanViewModel>()).Create();

            //act
            var result = new TransferEvidenceNotesViewModelMapTransfer(noteData, recipientData, organisationId, model);

            //assert
            result.OrganisationId.Should().Be(organisationId);
            result.RecipientData.Should().BeEquivalentTo(recipientData);
            result.TransferEvidenceNoteData.Should().Be(noteData);
            result.ExistingTransferEvidenceNoteCategoriesViewModel.Should().Be(model);
        }

        [Fact]
        public void TransferEvidenceNotesViewModelMapTransfer_GivenCategoryConstructorAndTransferNoteDataIsNull_ArgumentNullExceptionExpected()
        {
            //arrange
            var recipientData = TestFixture.CreateMany<OrganisationSchemeData>().ToList();
            var organisationId = TestFixture.Create<Guid>();

            //act
            var exception = Record.Exception(() => new TransferEvidenceNotesViewModelMapTransfer(null, recipientData, organisationId, null));

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
            var recipientData = TestFixture.CreateMany<OrganisationSchemeData>().ToList();
            var noteData = TestFixture.Create<TransferEvidenceNoteData>();

            //act
            var exception = Record.Exception(() => new TransferEvidenceNotesViewModelMapTransfer(noteData, recipientData, Guid.Empty, null));

            //assert
            exception.Should().BeOfType<ArgumentException>();
        }
    }
}
