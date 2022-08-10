namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Mapping.ToViewModel
{
    using System;
    using AutoFixture;
    using Core.AatfEvidence;
    using EA.Prsd.Core;
    using FluentAssertions;
    using Web.Areas.Aatf.Mappings.ToViewModel;
    using Web.ViewModels.Shared;
    using Weee.Tests.Core;
    using Xunit;

    public class EvidenceNotesViewModelTransferTests : SimpleUnitTestBase
    {
        private const int PageNumber = 2;

        [Fact]
        public void EvidenceNotesViewModelTransfer_GiveListOfNotesIsNull_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => new EvidenceNotesViewModelTransfer(Guid.NewGuid(), 
                Guid.NewGuid(), 
                null, 
                SystemTime.Now, 
                null,
                PageNumber));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void EvidenceNotesViewModelTransfer_GivenOrganisationGuidIsEmpty_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => new EvidenceNotesViewModelTransfer(Guid.Empty, Guid.NewGuid(), TestFixture.Create<EvidenceNoteSearchDataResult>(), SystemTime.Now, null, PageNumber));

            //assert
            exception.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void EvidenceNotesViewModelTransfer_GivenEvidenceNoteIdGuidIsEmpty_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => new EvidenceNotesViewModelTransfer(Guid.NewGuid(), Guid.Empty, TestFixture.Create<EvidenceNoteSearchDataResult>(), SystemTime.Now, null, PageNumber));

            //assert
            exception.Should().BeOfType<ArgumentException>();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void EvidenceNotesViewModelTransfer_Constructor_GivenPageNumberIsLessThanOne_ShouldThrowAnException(int pageNumber)
        {
            //act
            var exception = Record.Exception(() => new EvidenceNotesViewModelTransfer(TestFixture.Create<Guid>(), TestFixture.Create<Guid>(), TestFixture.Create<EvidenceNoteSearchDataResult>(), SystemTime.Now, null, pageNumber));

            // assert
            exception.Should().BeOfType<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void EvidenceNotesViewModelTransfer_Constructor_GivenValues_PropertiesShouldBeSet()
        {
            //arrange
            var organisationId = TestFixture.Create<Guid>();
            var aatfId = TestFixture.Create<Guid>();
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();
            var currentDate = TestFixture.Create<DateTime>();
            var model = TestFixture.Create<ManageEvidenceNoteViewModel>();

            //act
            var mapper = new EvidenceNotesViewModelTransfer(organisationId, aatfId, noteData, currentDate, model, PageNumber);

            //assert
            mapper.AatfId.Should().Be(aatfId);
            mapper.CurrentDate.Should().Be(currentDate);
            mapper.ManageEvidenceNoteViewModel.Should().Be(model);
            mapper.NoteData.Should().Be(noteData);
            mapper.PageNumber.Should().Be(PageNumber);
        }
    }
}
