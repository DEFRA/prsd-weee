namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Mapping
{
    using AutoFixture;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels;
    using EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Web.Areas.Aatf.ViewModels;
    using Web.ViewModels.Shared;
    using Web.ViewModels.Shared.Mapping;
    using Xunit;

    public class ViewAndTransferEvidenceViewModelMapTests
    {
        private readonly ViewAndTransferEvidenceViewModelMap viewAndTransferEvidenceViewModelMap;
        private readonly Fixture fixture;
        private readonly IMapper mapper;

        public ViewAndTransferEvidenceViewModelMapTests()
        {
            mapper = A.Fake<IMapper>();

            viewAndTransferEvidenceViewModelMap = new ViewAndTransferEvidenceViewModelMap(mapper);

            fixture = new Fixture();
        }

        [Fact]
        public void ViewAndTransferEvidenceViewModelMap_ShouldBeDerivedFromListOfNotesViewModelBase()
        {
            typeof(ViewAndTransferEvidenceViewModelMap).Should()
                .BeDerivedFrom<ListOfNotesViewModelBase<SchemeViewAndTransferManageEvidenceSchemeViewModel>>();
        }

        [Fact]
        public void Map_GiveListOfNotesIsNull_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => new ViewAndTransferEvidenceViewModelMapTransfer(Guid.NewGuid(), 
                null,
                fixture.Create<SchemePublicInfo>(), 
                fixture.Create<DateTime>(),
                fixture.Create<ManageEvidenceNoteViewModel>()));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenOrganisationGuidIsEmpty_ArgumentExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => new ViewAndTransferEvidenceViewModelMapTransfer(Guid.Empty, 
                fixture.CreateMany<EvidenceNoteData>().ToList(),
                fixture.Create<SchemePublicInfo>(), 
                fixture.Create<DateTime>(),
                fixture.Create<ManageEvidenceNoteViewModel>()));

            //assert
            exception.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void Map_GivenSchemeNameIsNull_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => new ViewAndTransferEvidenceViewModelMapTransfer(Guid.NewGuid(), fixture.CreateMany<EvidenceNoteData>().ToList(), 
                null, 
                fixture.Create<DateTime>(),
                fixture.Create<ManageEvidenceNoteViewModel>()));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenListOfEvidenceNotes_MapperShouldBeCalled()
        {
            //arrange
            var notes = new List<EvidenceNoteData>
            {
                 fixture.Create<EvidenceNoteData>(),
                 fixture.Create<EvidenceNoteData>(),
                 fixture.Create<EvidenceNoteData>()
            };

            var organisationId = Guid.NewGuid();

            var transfer = new ViewAndTransferEvidenceViewModelMapTransfer(organisationId, 
                notes,
                fixture.Create<SchemePublicInfo>(), 
                fixture.Create<DateTime>(),
                fixture.Create<ManageEvidenceNoteViewModel>());

            //act
            viewAndTransferEvidenceViewModelMap.Map(transfer);

            // assert 
            A.CallTo(() => mapper.Map<List<EvidenceNoteRowViewModel>>(notes)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Map_GivenSchemeNameAndOrganisationId_PropertiesShouldBeSet()
        {
            //arrange
            var organisationId = fixture.Create<Guid>();
            var scheme = fixture.Create<SchemePublicInfo>();

            var transfer = new ViewAndTransferEvidenceViewModelMapTransfer(organisationId,
                fixture.CreateMany<EvidenceNoteData>().ToList(),
                scheme,
                fixture.Create<DateTime>(),
                fixture.Create<ManageEvidenceNoteViewModel>());

            //act
            var result = viewAndTransferEvidenceViewModelMap.Map(transfer);

            //assert
            result.OrganisationId.Should().Be(organisationId);
            result.Scheme.Should().Be(scheme);
        }

        [Fact]
        public void Map_GivenAnEmptyListOfEvidenceNotes_MapperShouldNotBeCalled()
        {
            //arrange
            var notes = new List<EvidenceNoteData>();

            var organisationId = Guid.NewGuid();

            var transfer = new ViewAndTransferEvidenceViewModelMapTransfer(organisationId, 
                notes,
                fixture.Create<SchemePublicInfo>(), 
                fixture.Create<DateTime>(),
                fixture.Create<ManageEvidenceNoteViewModel>());

            //act
            viewAndTransferEvidenceViewModelMap.Map(transfer);

            // assert 
            A.CallTo(() => mapper.Map<EvidenceNoteRowViewModel>(A<EvidenceNoteRowViewModel>._)).MustHaveHappened(0, Times.Exactly);
        }

        [Fact]
        public void Map_GivenAnEmptyListOfEvidenceNotes_MustReturnAnEmptyModel()
        {
            //arrange
            var notes = new List<EvidenceNoteData>();

            var organisationId = Guid.NewGuid();

            var transfer = new ViewAndTransferEvidenceViewModelMapTransfer(organisationId, 
                notes,
                fixture.Create<SchemePublicInfo>(), 
                fixture.Create<DateTime>(),
                fixture.Create<ManageEvidenceNoteViewModel>());

            //act
            var result = viewAndTransferEvidenceViewModelMap.Map(transfer);

            // assert 
            result.EvidenceNotesDataList.Should().BeNullOrEmpty();
        }

        [Fact]
        public void Map_GivenListOfEvidenceNoteData_ShouldReturnMappedData()
        {
            //arrange
            var notes = fixture.CreateMany<EvidenceNoteData>().ToList();

            var returnedNotes = new List<EvidenceNoteRowViewModel>
            {
                 fixture.Create<EvidenceNoteRowViewModel>(),
                 fixture.Create<EvidenceNoteRowViewModel>(),
                 fixture.Create<EvidenceNoteRowViewModel>()
            };

            var model = new SchemeViewAndTransferManageEvidenceSchemeViewModel
            {
                EvidenceNotesDataList = returnedNotes
            };

            var organisationId = Guid.NewGuid();

            var transfer = new ViewAndTransferEvidenceViewModelMapTransfer(organisationId, 
                notes,
                fixture.Create<SchemePublicInfo>(), 
                fixture.Create<DateTime>(),
                fixture.Create<ManageEvidenceNoteViewModel>());

            A.CallTo(() => mapper.Map<List<EvidenceNoteRowViewModel>>(A<List<EvidenceNoteData>>._)).Returns(returnedNotes);

            //act
            var result = viewAndTransferEvidenceViewModelMap.Map(transfer);

            // assert
            result.EvidenceNotesDataList.Should().NotBeEmpty();
            result.EvidenceNotesDataList.Should().BeEquivalentTo(returnedNotes);
        }

        [Fact]
        public void Map_GivenNoApprovedEvidenceNotes_DisplayTransferButtonShouldBeSetToFalse()
        {
            //arrange
            var notes = new List<EvidenceNoteData>
            {
                fixture.Build<EvidenceNoteData>().With(a => a.Status, NoteStatus.Draft).Create(),
                fixture.Build<EvidenceNoteData>().With(a => a.Status, NoteStatus.Rejected).Create(),
                fixture.Build<EvidenceNoteData>().With(a => a.Status, NoteStatus.Submitted).Create(),
                fixture.Build<EvidenceNoteData>().With(a => a.Status, NoteStatus.Void).Create()
            };

            //act
            var result = viewAndTransferEvidenceViewModelMap.Map(new ViewAndTransferEvidenceViewModelMapTransfer(fixture.Create<Guid>(),
                notes,
                fixture.Create<SchemePublicInfo>(), 
                fixture.Create<DateTime>(),
                fixture.Create<ManageEvidenceNoteViewModel>()));

            //assert
            result.DisplayTransferButton.Should().BeFalse();
        }

        [Fact]
        public void Map_GivenApprovedEvidenceNotes_DisplayTransferButtonShouldBeSetToFalse()
        {
            //arrange
            var notes = new List<EvidenceNoteData>
            {
                fixture.Build<EvidenceNoteData>().With(a => a.Status, NoteStatus.Approved).Create(),
            };

            //act
            var result = viewAndTransferEvidenceViewModelMap.Map(new ViewAndTransferEvidenceViewModelMapTransfer(fixture.Create<Guid>(),
                notes,
                fixture.Create<SchemePublicInfo>(), 
                fixture.Create<DateTime>(),
                fixture.Create<ManageEvidenceNoteViewModel>()));

            //assert
            result.DisplayTransferButton.Should().BeTrue();
        }

        [Fact]
        public void Map_GivenCurrentDate_ComplianceYearsListShouldBeReturned()
        {
            //arrange
            var notes = fixture.CreateMany<EvidenceNoteData>().ToList();
            var model = fixture.Create<ManageEvidenceNoteViewModel>();
            var date = new DateTime(2022, 1, 1);

            //act
            var result = viewAndTransferEvidenceViewModelMap.MapBase(notes, date, model);

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
            var notes = fixture.CreateMany<EvidenceNoteData>().ToList();
            var date = new DateTime(year, 1, 1);

            //act
            var result = viewAndTransferEvidenceViewModelMap.MapBase(notes, date, null);

            //assert
            result.ManageEvidenceNoteViewModel.SelectedComplianceYear.Should().Be(year);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Map_GivenCurrentDateAndManageEvidenceViewModelSelectedComplianceYearIsNotGreaterThanZero_SelectedComplianceYearShouldBeSet(int complianceYear)
        {
            //arrange
            var notes = fixture.CreateMany<EvidenceNoteData>().ToList();
            var date = new DateTime(2022, 1, 1);
            var model = fixture.Build<ManageEvidenceNoteViewModel>()
                .With(m => m.SelectedComplianceYear, complianceYear).Create();

            //act
            var result = viewAndTransferEvidenceViewModelMap.MapBase(notes, date, model);

            //assert
            result.ManageEvidenceNoteViewModel.SelectedComplianceYear.Should().Be(2022);
        }

        [Fact]
        public void Map_GivenCurrentDateAndManageEvidenceViewModelWithSelectedComplianceYear_SelectedComplianceYearShouldBeSet()
        {
            //arrange
            var notes = fixture.CreateMany<EvidenceNoteData>().ToList();
            var date = new DateTime(2022, 1, 1);
            var model = fixture.Build<ManageEvidenceNoteViewModel>()
                .With(m => m.SelectedComplianceYear, 2021).Create();

            //act
            var result = viewAndTransferEvidenceViewModelMap.MapBase(notes, date, model);

            //assert
            result.ManageEvidenceNoteViewModel.SelectedComplianceYear.Should().Be(2021);
        }
    }
}
