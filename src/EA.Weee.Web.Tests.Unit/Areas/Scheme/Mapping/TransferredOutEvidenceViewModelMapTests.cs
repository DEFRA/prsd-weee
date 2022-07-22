namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Mapping
{
    using AutoFixture;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels;
    using EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Web.ViewModels.Shared;
    using Web.ViewModels.Shared.Mapping;
    using Weee.Tests.Core;
    using Xunit;

    public class TransferredOutEvidenceViewModelMapTests : SimpleUnitTestBase
    {
        private readonly TransferredOutEvidenceViewModelMap transferredOutEvidenceViewModelMap;
        private readonly IMapper mapper;

        public TransferredOutEvidenceViewModelMapTests()
        {
            mapper = A.Fake<IMapper>();

            transferredOutEvidenceViewModelMap = new TransferredOutEvidenceViewModelMap(mapper);
        }

        [Fact]
        public void TransferredOutEvidenceViewModelMap_ShouldBeDerivedFromListOfNotesViewModelBase()
        {
            typeof(TransferredOutEvidenceViewModelMap).Should()
                .BeDerivedFrom<ListOfNotesViewModelBase<TransferredOutEvidenceNotesSchemeViewModel>>();
        }

        [Fact]
        public void Map_GiveListOfNotesIsNull_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => new TransferredOutEvidenceNotesViewModelMapTransfer(Guid.NewGuid(),
                null,
                TestFixture.Create<string>(),
                TestFixture.Create<DateTime>(),
                TestFixture.Create<ManageEvidenceNoteViewModel>()));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenOrganisationGuidIsEmpty_ArgumentExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => new TransferredOutEvidenceNotesViewModelMapTransfer(Guid.Empty,
                TestFixture.Create<EvidenceNoteSearchDataResult>(),
                TestFixture.Create<string>(),
                TestFixture.Create<DateTime>(),
                TestFixture.Create<ManageEvidenceNoteViewModel>()));

            //assert
            exception.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void Map_GivenSchemeNameIsNull_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => new TransferredOutEvidenceNotesViewModelMapTransfer(Guid.NewGuid(),
                TestFixture.Create<EvidenceNoteSearchDataResult>(),
                null,
                TestFixture.Create<DateTime>(),
                TestFixture.Create<ManageEvidenceNoteViewModel>()));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenSchemeNameAndOrganisationId_PropertiesShouldBeSet()
        {
            //arrange
            var organisationId = TestFixture.Create<Guid>();
            var schemeName = TestFixture.Create<string>();

            var transfer = new TransferredOutEvidenceNotesViewModelMapTransfer(organisationId,
                TestFixture.Create<EvidenceNoteSearchDataResult>(),
                schemeName,
                TestFixture.Create<DateTime>(),
                TestFixture.Create<ManageEvidenceNoteViewModel>());

            //act
            var result = transferredOutEvidenceViewModelMap.Map(transfer);

            //assert
            result.OrganisationId.Should().Be(organisationId);
            result.SchemeName.Should().Be(schemeName);
        }

        [Fact]
        public void Map_GivenListOfEvidenceNotes_MapperShouldBeCalled()
        {
            //arrange
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();
            var organisationId = Guid.NewGuid();

            var transfer = new TransferredOutEvidenceNotesViewModelMapTransfer(organisationId,
                noteData,
                TestFixture.Create<string>(),
                TestFixture.Create<DateTime>(),
                TestFixture.Create<ManageEvidenceNoteViewModel>());

            //act
            transferredOutEvidenceViewModelMap.Map(transfer);

            // assert 
            A.CallTo(() => mapper.Map<List<EvidenceNoteRowViewModel>>(A<List<EvidenceNoteData>>.That.Matches(e =>
                e.SequenceEqual(noteData.Results.ToList())))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Map_GivenAnEmptyListOfEvidenceNotes_MapperShouldNotBeCalled()
        {
            //arrange
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();
            var organisationId = Guid.NewGuid();

            var transfer = new TransferredOutEvidenceNotesViewModelMapTransfer(organisationId,
                noteData,
                TestFixture.Create<string>(),
                TestFixture.Create<DateTime>(),
                TestFixture.Create<ManageEvidenceNoteViewModel>());

            //act
            transferredOutEvidenceViewModelMap.Map(transfer);

            // assert 
            A.CallTo(() => mapper.Map<EvidenceNoteRowViewModel>(A<EvidenceNoteRowViewModel>._)).MustHaveHappened(0, Times.Exactly);
        }

        [Fact]
        public void Map_GivenAnEmptyListOfEvidenceNotes_MustReturnAnEmptyModel()
        {
            //arrange
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();
            var organisationId = Guid.NewGuid();

            var transfer = new TransferredOutEvidenceNotesViewModelMapTransfer(organisationId,
                noteData,
                TestFixture.Create<string>(),
                TestFixture.Create<DateTime>(),
                TestFixture.Create<ManageEvidenceNoteViewModel>());

            //act
            var result = transferredOutEvidenceViewModelMap.Map(transfer);

            // assert 
            result.EvidenceNotesDataList.Should().BeNullOrEmpty();
        }

        [Fact]
        public void Map_GivenListOfEvidenceNoteData_ShouldReturnMappedData()
        {
            //arrange
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();

            var returnedNotes = new List<EvidenceNoteRowViewModel>
            {
                 TestFixture.Create<EvidenceNoteRowViewModel>(),
                 TestFixture.Create<EvidenceNoteRowViewModel>(),
                 TestFixture.Create<EvidenceNoteRowViewModel>()
            };

            var organisationId = Guid.NewGuid();

            var transfer = new TransferredOutEvidenceNotesViewModelMapTransfer(organisationId,
                noteData,
                TestFixture.Create<string>(),
                TestFixture.Create<DateTime>(),
                TestFixture.Create<ManageEvidenceNoteViewModel>());

            A.CallTo(() => mapper.Map<List<EvidenceNoteRowViewModel>>(A<List<EvidenceNoteData>>._)).Returns(returnedNotes);

            //act
            var result = transferredOutEvidenceViewModelMap.Map(transfer);

            // assert
            result.EvidenceNotesDataList.Should().NotBeEmpty();
            result.EvidenceNotesDataList.Should().BeEquivalentTo(returnedNotes);
        }

        [Fact]
        public void Map_GivenCurrentDate_ComplianceYearsListShouldBeReturned()
        {
            //arrange
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();
            var model = TestFixture.Create<ManageEvidenceNoteViewModel>();
            var date = new DateTime(2022, 1, 1);

            //act
            var result = transferredOutEvidenceViewModelMap.MapBase(noteData, date, model);

            //assert
            result.ManageEvidenceNoteViewModel.ComplianceYearList.Count().Should().Be(3);
            result.ManageEvidenceNoteViewModel.ComplianceYearList.ElementAt(0).Should().Be(2022);
            result.ManageEvidenceNoteViewModel.ComplianceYearList.ElementAt(1).Should().Be(2021);
            result.ManageEvidenceNoteViewModel.ComplianceYearList.ElementAt(2).Should().Be(2020);
        }

        [Theory]
        [InlineData(2021)]
        [InlineData(2020)]
        [InlineData(2022)]
        public void Map_GivenCurrentDateAndManageEvidenceViewModelIsNull_SelectedComplianceYearShouldBeSet(int year)
        {
            //arrange
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();
            var date = new DateTime(year, 1, 1);

            //act
            var result = transferredOutEvidenceViewModelMap.MapBase(noteData, date, null);

            //assert
            result.ManageEvidenceNoteViewModel.SelectedComplianceYear.Should().Be(year);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Map_GivenCurrentDateAndManageEvidenceViewModelSelectedComplianceYearIsNotGreaterThanZero_SelectedComplianceYearShouldBeSet(int selectedComplianceYear)
        {
            //arrange
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();
            var date = new DateTime(2022, 1, 1);
            var model = TestFixture.Build<ManageEvidenceNoteViewModel>()
                .With(m => m.SelectedComplianceYear, selectedComplianceYear).Create();

            //act
            var result = transferredOutEvidenceViewModelMap.MapBase(noteData, date, model);

            //assert
            result.ManageEvidenceNoteViewModel.SelectedComplianceYear.Should().Be(2022);
        }

        [Fact]
        public void Map_GivenCurrentDateAndManageEvidenceViewModelWithSelectedComplianceYear_SelectedComplianceYearShouldBeSet()
        {
            //arrange
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();
            var date = new DateTime(2022, 1, 1);
            var model = TestFixture.Build<ManageEvidenceNoteViewModel>()
                .With(m => m.SelectedComplianceYear, 2021).Create();

            //act
            var result = transferredOutEvidenceViewModelMap.MapBase(noteData, date, model);

            //assert
            result.ManageEvidenceNoteViewModel.SelectedComplianceYear.Should().Be(2021);
        }

        [Fact]
        public void Map_GivenListOfEvidenceNoteRowViewModel_DisplayEditLinkPropertyShouldBeSet()
        {
            //arrange
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();

            var returnedNotes = new List<EvidenceNoteRowViewModel>
            {
                TestFixture.Build<EvidenceNoteRowViewModel>().With(e => e.Status, NoteStatus.Draft).Create(),
                TestFixture.Build<EvidenceNoteRowViewModel>().With(e => e.Status, NoteStatus.Approved).Create(),
                TestFixture.Build<EvidenceNoteRowViewModel>().With(e => e.Status, NoteStatus.Rejected).Create(),
                TestFixture.Build<EvidenceNoteRowViewModel>().With(e => e.Status, NoteStatus.Returned).Create(),
                TestFixture.Build<EvidenceNoteRowViewModel>().With(e => e.Status, NoteStatus.Submitted).Create(),
                TestFixture.Build<EvidenceNoteRowViewModel>().With(e => e.Status, NoteStatus.Void).Create()
            };

            var organisationId = Guid.NewGuid();

            var transfer = new TransferredOutEvidenceNotesViewModelMapTransfer(organisationId,
                noteData,
                TestFixture.Create<string>(),
                TestFixture.Create<DateTime>(),
                TestFixture.Create<ManageEvidenceNoteViewModel>());

            A.CallTo(() => mapper.Map<List<EvidenceNoteRowViewModel>>(A<List<EvidenceNoteData>>._)).Returns(returnedNotes);

            //act
            var result = transferredOutEvidenceViewModelMap.Map(transfer);

            // assert
            var acceptedList = new List<NoteStatus>() { NoteStatus.Draft };

            foreach (var evidenceNoteRowViewModel in result.EvidenceNotesDataList.Where(e => acceptedList.Contains(e.Status)))
            {
                evidenceNoteRowViewModel.DisplayEditLink.Should().BeTrue();
            }
            foreach (var evidenceNoteRowViewModel in result.EvidenceNotesDataList.Where(e => !acceptedList.Contains(e.Status)))
            {
                evidenceNoteRowViewModel.DisplayEditLink.Should().BeFalse();
            }
        }
    }
}
