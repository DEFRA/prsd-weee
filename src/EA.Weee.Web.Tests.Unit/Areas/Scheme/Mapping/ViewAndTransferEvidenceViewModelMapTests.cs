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
    using Core.Shared;
    using Services;
    using Web.ViewModels.Shared;
    using Web.ViewModels.Shared.Mapping;
    using Weee.Tests.Core;
    using Weee.Tests.Core.DataHelpers;
    using Xunit;

    public class ViewAndTransferEvidenceViewModelMapTests : SimpleUnitTestBase
    {
        private readonly ViewAndTransferEvidenceViewModelMap viewAndTransferEvidenceViewModelMap;
        private readonly IMapper mapper;
        private readonly ConfigurationService configurationService;

        public ViewAndTransferEvidenceViewModelMapTests()
        {
            mapper = A.Fake<IMapper>();
            configurationService = A.Fake<ConfigurationService>();
            A.CallTo(() => configurationService.CurrentConfiguration.DefaultPagingPageSize).Returns(25);

            viewAndTransferEvidenceViewModelMap = new ViewAndTransferEvidenceViewModelMap(mapper, configurationService);
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
            var exception = Record.Exception(() => new SchemeTabViewModelMapTransfer(Guid.NewGuid(), 
                null,
                TestFixture.Create<SchemePublicInfo>(), 
                TestFixture.Create<DateTime>(),
                TestFixture.Create<ManageEvidenceNoteViewModel>(),
                TestFixture.Create<int>()));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenOrganisationGuidIsEmpty_ArgumentExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => new SchemeTabViewModelMapTransfer(Guid.Empty,
                TestFixture.Create<EvidenceNoteSearchDataResult>(),
                TestFixture.Create<SchemePublicInfo>(),
                TestFixture.Create<DateTime>(),
                TestFixture.Create<ManageEvidenceNoteViewModel>(),
                TestFixture.Create<int>()));

            //assert
            exception.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void Map_GivenListOfEvidenceNotes_MapperShouldBeCalled()
        {
            //arrange
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();

            var organisationId = Guid.NewGuid();

            var transfer = new SchemeTabViewModelMapTransfer(organisationId,
                noteData,
                TestFixture.Create<SchemePublicInfo>(), 
                TestFixture.Create<DateTime>(),
                TestFixture.Create<ManageEvidenceNoteViewModel>(),
                TestFixture.Create<int>());

            //act
            viewAndTransferEvidenceViewModelMap.Map(transfer);

            // assert 
            A.CallTo(() => mapper.Map<List<EvidenceNoteRowViewModel>>(A<List<EvidenceNoteData>>.That.Matches(e =>
                e.SequenceEqual(noteData.Results.ToList())))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Map_GivenSchemeNameAndOrganisationId_PropertiesShouldBeSet()
        {
            //arrange
            var organisationId = TestFixture.Create<Guid>();
            var scheme = TestFixture.Create<SchemePublicInfo>();

            var transfer = new SchemeTabViewModelMapTransfer(organisationId,
                TestFixture.Create<EvidenceNoteSearchDataResult>(),
                scheme,
                TestFixture.Create<DateTime>(),
                TestFixture.Create<ManageEvidenceNoteViewModel>(),
                TestFixture.Create<int>());

            //act
            var result = viewAndTransferEvidenceViewModelMap.Map(transfer);

            //assert
            result.OrganisationId.Should().Be(organisationId);
        }

        [Fact]
        public void Map_GivenAnEmptyListOfEvidenceNotes_MapperShouldNotBeCalled()
        {
            //arrange
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();

            var organisationId = Guid.NewGuid();

            var transfer = new SchemeTabViewModelMapTransfer(organisationId,
                noteData,
                TestFixture.Create<SchemePublicInfo>(),
                TestFixture.Create<DateTime>(),
                TestFixture.Create<ManageEvidenceNoteViewModel>(),
                TestFixture.Create<int>());

            //act
            viewAndTransferEvidenceViewModelMap.Map(transfer);

            // assert 
            A.CallTo(() => mapper.Map<EvidenceNoteRowViewModel>(A<EvidenceNoteRowViewModel>._)).MustHaveHappened(0, Times.Exactly);
        }

        [Fact]
        public void Map_GivenAnEmptyListOfEvidenceNotes_MustReturnAnEmptyModel()
        {
            //arrange
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();
            var organisationId = Guid.NewGuid();

            var transfer = new SchemeTabViewModelMapTransfer(organisationId,
                noteData,
                TestFixture.Create<SchemePublicInfo>(),
                TestFixture.Create<DateTime>(),
                TestFixture.Create<ManageEvidenceNoteViewModel>(),
                TestFixture.Create<int>());

            //act
            var result = viewAndTransferEvidenceViewModelMap.Map(transfer);

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

            var transfer = new SchemeTabViewModelMapTransfer(organisationId,
                noteData,
                TestFixture.Create<SchemePublicInfo>(),
                TestFixture.Create<DateTime>(),
                TestFixture.Create<ManageEvidenceNoteViewModel>(),
                TestFixture.Create<int>());

            A.CallTo(() => mapper.Map<List<EvidenceNoteRowViewModel>>(A<List<EvidenceNoteData>>._)).Returns(returnedNotes);

            //act
            var result = viewAndTransferEvidenceViewModelMap.Map(transfer);

            // assert
            result.EvidenceNotesDataList.Should().NotBeEmpty();
            result.EvidenceNotesDataList.Should().BeEquivalentTo(returnedNotes);
        }

        [Fact]
        public void Map_GivenListOfEvidenceNoteData_ShouldReturnMappedDataAsPagedList()
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
            var pageNumber = TestFixture.Create<int>();
            var pageSize = TestFixture.Create<int>();

            var transfer = new SchemeTabViewModelMapTransfer(organisationId,
                noteData,
                TestFixture.Create<SchemePublicInfo>(),
                TestFixture.Create<DateTime>(),
                TestFixture.Create<ManageEvidenceNoteViewModel>(),
                pageNumber);

            A.CallTo(() => mapper.Map<List<EvidenceNoteRowViewModel>>(A<List<EvidenceNoteData>>._)).Returns(returnedNotes);
            A.CallTo(() => configurationService.CurrentConfiguration.DefaultPagingPageSize).Returns(pageSize);

            //act
            var result = viewAndTransferEvidenceViewModelMap.Map(transfer);

            // assert
            result.EvidenceNotesDataList.Should().NotBeEmpty();
            result.EvidenceNotesDataList.Should().BeEquivalentTo(returnedNotes);
            result.EvidenceNotesDataList.PageNumber.Should().Be(pageNumber);
            result.EvidenceNotesDataList.PageSize.Should().Be(pageSize);
        }

        [Fact]
        public void Map_GivenNoApprovedEvidenceNotesAndSchemeCanManageEvidence_DisplayTransferButtonShouldBeSetToFalse()
        {
            //arrange
            var notes = new List<EvidenceNoteData>
            {
                TestFixture.Build<EvidenceNoteData>().With(a => a.Status, NoteStatus.Draft).With(a => a.WasteType, WasteType.Household).Create(),
                TestFixture.Build<EvidenceNoteData>().With(a => a.Status, NoteStatus.Rejected).With(a => a.WasteType, WasteType.NonHousehold).Create(),
                TestFixture.Build<EvidenceNoteData>().With(a => a.Status, NoteStatus.Submitted).With(a => a.WasteType, WasteType.NonHousehold).Create(),
                TestFixture.Build<EvidenceNoteData>().With(a => a.Status, NoteStatus.Void).With(a => a.WasteType, WasteType.Household).Create()
            };
            var noteData = new EvidenceNoteSearchDataResult(notes, notes.Count);
            var schemeInfo = TestFixture.Build<SchemePublicInfo>().With(s => s.Status, SchemeStatus.Approved).Create();
            var currentDate = new DateTime(2020, 1, 1);
            var model = TestFixture.Build<ManageEvidenceNoteViewModel>().With(m => m.SelectedComplianceYear, currentDate.Year).Create();

            //act
            var result = viewAndTransferEvidenceViewModelMap.Map(new SchemeTabViewModelMapTransfer(TestFixture.Create<Guid>(),
                noteData,
                schemeInfo,
                currentDate,
                model,
                TestFixture.Create<int>()));

            //assert
            result.DisplayTransferButton.Should().BeFalse();
        }

        [Theory]
        [ClassData(typeof(SchemeStatusCoreData))]
        public void Map_GivenApprovedEvidenceNotesAndSchemeIsNotWithdrawnAndComplianceWindowIsOpen_DisplayTransferButtonShouldBeSetToTrue(SchemeStatus status)
        {
            if (status == SchemeStatus.Withdrawn)
            {
                return;
            }

            var currentDate = new DateTime(2022, 1, 1);
            var manageEvidenceModel = TestFixture.Build<ManageEvidenceNoteViewModel>()
                .With(m => m.SelectedComplianceYear, 2022).Create();

            //arrange
            var notes = new List<EvidenceNoteData>
            {
                TestFixture.Build<EvidenceNoteData>().With(a => a.Status, NoteStatus.Approved).With(a => a.WasteType, WasteType.Household).Create(),
            };
            var scheme = TestFixture.Build<SchemePublicInfo>()
                .With(s => s.Status, status)
                .Create();
            var noteData = TestFixture.Build<EvidenceNoteSearchDataResult>()
                .With(e => e.Results, notes).Create();

            //act
            var result = viewAndTransferEvidenceViewModelMap.Map(new SchemeTabViewModelMapTransfer(TestFixture.Create<Guid>(),
                noteData,
                scheme,
                currentDate,
                manageEvidenceModel,
                TestFixture.Create<int>()));

            //assert
            result.DisplayTransferButton.Should().BeTrue();
        }

        [Theory]
        [ClassData(typeof(SchemeStatusCoreData))]
        public void Map_GivenApprovedEvidenceNotesAndSchemeIsNotWithdrawnAndComplianceWindowIsOpenAndExistingModelIsNull_DisplayTransferButtonShouldBeSetToTrue(SchemeStatus status)
        {
            if (status == SchemeStatus.Withdrawn)
            {
                return;
            }

            var currentDate = new DateTime(2022, 1, 1);
          
            //arrange
            var notes = new List<EvidenceNoteData>
            {
                TestFixture.Build<EvidenceNoteData>()
                .With(a => a.Status, NoteStatus.Approved)
                .With(a => a.WasteType, WasteType.Household)
                .Create(),
            };
            var scheme = TestFixture.Build<SchemePublicInfo>()
                .With(s => s.Status, status)
                .Create();
            var noteData = TestFixture.Build<EvidenceNoteSearchDataResult>()
                .With(e => e.Results, notes).Create();

            //act
            var result = viewAndTransferEvidenceViewModelMap.Map(new SchemeTabViewModelMapTransfer(TestFixture.Create<Guid>(),
                noteData,
                scheme,
                currentDate,
                null,
                TestFixture.Create<int>()));

            //assert
            result.DisplayTransferButton.Should().BeTrue();
        }

        [Theory]
        [ClassData(typeof(SchemeStatusCoreData))]
        public void Map_GivenApprovedEvidenceNotesAndSchemeIsNotWithdrawnAndComplianceWindowIsOpenAndExistingModelIsNull_DisplayTransferButtonShouldBeSetToFalse(SchemeStatus status)
        {
            if (status == SchemeStatus.Withdrawn)
            {
                return;
            }

            var currentDate = new DateTime(2022, 1, 1);

            //arrange
            var notes = new List<EvidenceNoteData>
            {
                TestFixture.Build<EvidenceNoteData>()
                .With(a => a.Status, NoteStatus.Approved)
                .With(a => a.WasteType, WasteType.NonHousehold)
                .Create(),
            };
            var scheme = TestFixture.Build<SchemePublicInfo>()
                .With(s => s.Status, status)
                .Create();
            var noteData = TestFixture.Build<EvidenceNoteSearchDataResult>()
                .With(e => e.Results, notes).Create();

            //act
            var result = viewAndTransferEvidenceViewModelMap.Map(new SchemeTabViewModelMapTransfer(TestFixture.Create<Guid>(),
                noteData,
                scheme,
                currentDate,
                null,
                TestFixture.Create<int>()));

            //assert
            result.DisplayTransferButton.Should().BeFalse();
        }

        [Fact]
        public void Map_GivenApprovedEvidenceNotesAndSchemeIsWithDrawn_DisplayTransferButtonShouldBeSetToFalse()
        {
            //arrange
            var currentDate = new DateTime(2022, 1, 1);
            var manageEvidenceModel = TestFixture.Build<ManageEvidenceNoteViewModel>()
                .With(m => m.SelectedComplianceYear, 2022).Create();
            var notes = new List<EvidenceNoteData>
            {
                TestFixture.Build<EvidenceNoteData>()
                .With(a => a.Status, NoteStatus.Approved)
                .With(a => a.WasteType, WasteType.Household).Create(),
            };
            var scheme = TestFixture.Build<SchemePublicInfo>().With(s => s.Status, SchemeStatus.Withdrawn).Create();
            var noteData = TestFixture.Build<EvidenceNoteSearchDataResult>().With(e => e.Results, notes).Create();

            //act
            var result = viewAndTransferEvidenceViewModelMap.Map(new SchemeTabViewModelMapTransfer(TestFixture.Create<Guid>(),
                noteData,
                scheme,
                currentDate,
                manageEvidenceModel,
                TestFixture.Create<int>()));

            //assert
            result.DisplayTransferButton.Should().BeFalse();
        }

        [Theory]
        [ClassData(typeof(OutOfComplianceYearData))]
        public void Map_GivenApprovedEvidenceNotesAndSchemeIsNotWithDrawnButComplianceYearIsClosed_DisplayTransferButtonShouldBeSetToFalse(DateTime currentDate, int complianceYear)
        {
            //arrange
            var manageEvidenceModel = TestFixture.Build<ManageEvidenceNoteViewModel>()
                .With(m => m.SelectedComplianceYear, complianceYear).Create();
            var notes = new List<EvidenceNoteData>
            {
                TestFixture.Build<EvidenceNoteData>().With(a => a.Status, NoteStatus.Approved).With(a => a.WasteType, WasteType.Household).Create(),
            };
            var scheme = TestFixture.Build<SchemePublicInfo>().With(s => s.Status, SchemeStatus.Approved).Create();
            var noteData = TestFixture.Build<EvidenceNoteSearchDataResult>().With(e => e.Results, notes).Create();

            //act
            var result = viewAndTransferEvidenceViewModelMap.Map(new SchemeTabViewModelMapTransfer(TestFixture.Create<Guid>(),
                noteData,
                scheme,
                currentDate,
                manageEvidenceModel,
                TestFixture.Create<int>()));

            //assert
            result.DisplayTransferButton.Should().BeFalse();
        }

        [Fact]
        public void Map_GivenCurrentDate_ComplianceYearsListShouldBeReturned()
        {
            //arrange
            var model = TestFixture.Create<ManageEvidenceNoteViewModel>();
            var notes = TestFixture.CreateMany<EvidenceNoteData>().ToList();
            var date = new DateTime(2022, 1, 1);
            var noteData = TestFixture.Build<EvidenceNoteSearchDataResult>()
                .With(e => e.Results, notes).Create();

            var source = new SchemeTabViewModelMapTransfer(TestFixture.Create<Guid>(),
                noteData,
                TestFixture.Create<SchemePublicInfo>(),
                date,
                model,
                TestFixture.Create<int>());

            //act
            var result = viewAndTransferEvidenceViewModelMap.Map(source);

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
            var notes = TestFixture.CreateMany<EvidenceNoteData>().ToList();
            var date = new DateTime(year, 1, 1);
            var organisationId = TestFixture.Create<Guid>();
            var noteData = TestFixture.Build<EvidenceNoteSearchDataResult>()
                .With(e => e.Results, notes).Create();

            var transfer = new SchemeTabViewModelMapTransfer(organisationId,
                noteData,
                TestFixture.Create<SchemePublicInfo>(),
                date,
                null,
                TestFixture.Create<int>());

            //act
            var result = viewAndTransferEvidenceViewModelMap.Map(transfer);

            //assert
            result.ManageEvidenceNoteViewModel.SelectedComplianceYear.Should().Be(year);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Map_GivenCurrentDateAndManageEvidenceViewModelSelectedComplianceYearIsNotGreaterThanZero_SelectedComplianceYearShouldBeSet(int complianceYear)
        {
            //arrange
            var notes = TestFixture.CreateMany<EvidenceNoteData>().ToList();
            var date = new DateTime(2022, 1, 1);
            var model = TestFixture.Build<ManageEvidenceNoteViewModel>()
                .With(m => m.SelectedComplianceYear, complianceYear).Create();
            var organisationId = TestFixture.Create<Guid>();
            var noteData = TestFixture.Build<EvidenceNoteSearchDataResult>()
                .With(e => e.Results, notes).Create();

            var transfer = new SchemeTabViewModelMapTransfer(organisationId,
                noteData,
                TestFixture.Create<SchemePublicInfo>(),
                date,
                model,
                TestFixture.Create<int>());

            //act
            var result = viewAndTransferEvidenceViewModelMap.Map(transfer);

            //assert
            result.ManageEvidenceNoteViewModel.SelectedComplianceYear.Should().Be(2022);
        }

        [Fact]
        public void Map_GivenCurrentDateAndManageEvidenceViewModelWithSelectedComplianceYear_SelectedComplianceYearShouldBeSet()
        {
            //arrange
            var notes = TestFixture.CreateMany<EvidenceNoteData>().ToList();
            var date = new DateTime(2022, 1, 1);
            var model = TestFixture.Build<ManageEvidenceNoteViewModel>()
                .With(m => m.SelectedComplianceYear, 2021).Create();
            var organisationId = TestFixture.Create<Guid>();
            var noteData = TestFixture.Build<EvidenceNoteSearchDataResult>()
                .With(e => e.Results, notes).Create();

            var transfer = new SchemeTabViewModelMapTransfer(organisationId,
                noteData,
                TestFixture.Create<SchemePublicInfo>(),
                date,
                model,
                TestFixture.Create<int>());

            //act
            var result = viewAndTransferEvidenceViewModelMap.Map(transfer);

            //assert
            result.ManageEvidenceNoteViewModel.SelectedComplianceYear.Should().Be(2021);
        }

        [Fact]
        public void Map_GivenSourceWithScheme_SchemeShouldBeSet()
        {
            //arrange
            var organisationId = TestFixture.Create<Guid>();
            var scheme = TestFixture.Build<SchemePublicInfo>()
                .With(s => s.Status, SchemeStatus.Withdrawn).Create();
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();

            var transfer = new SchemeTabViewModelMapTransfer(organisationId,
                noteData,
                scheme,
                TestFixture.Create<DateTime>(),
                TestFixture.Create<ManageEvidenceNoteViewModel>(),
                TestFixture.Create<int>());

            //act
            var result = viewAndTransferEvidenceViewModelMap.Map(transfer);

            //assert
            result.SchemeInfo.Should().Be(scheme);
        }

        [Fact]
        public void Map_GivenSourceWithWithdrawnScheme_IsWithdrawnShouldBeTrue()
        {
            //arrange
            var organisationId = TestFixture.Create<Guid>();
            var scheme = TestFixture.Build<SchemePublicInfo>()
                .With(s => s.Status, SchemeStatus.Withdrawn).Create();

            var transfer = new SchemeTabViewModelMapTransfer(organisationId,
                TestFixture.Create<EvidenceNoteSearchDataResult>(),
                scheme,
                TestFixture.Create<DateTime>(),
                TestFixture.Create<ManageEvidenceNoteViewModel>(),
                TestFixture.Create<int>());

            //act
            var result = viewAndTransferEvidenceViewModelMap.Map(transfer);

            //assert
            result.IsWithdrawn.Should().BeTrue();
        }

        [Theory]
        [ClassData(typeof(SchemeStatusCoreData))]
        public void Map_GivenSourceWithNotWithdrawnScheme_IsWithdrawnShouldBeFalse(SchemeStatus status)
        {
            if (status == SchemeStatus.Withdrawn)
            {
                return;
            }

            //arrange
            var organisationId = TestFixture.Create<Guid>();
            var scheme = TestFixture.Build<SchemePublicInfo>()
                .With(s => s.Status, status).Create();

            var transfer = new SchemeTabViewModelMapTransfer(organisationId,
                TestFixture.Create<EvidenceNoteSearchDataResult>(),
                scheme,
                TestFixture.Create<DateTime>(),
                TestFixture.Create<ManageEvidenceNoteViewModel>(),
                TestFixture.Create<int>());

            //act
            var result = viewAndTransferEvidenceViewModelMap.Map(transfer);

            //assert
            result.IsWithdrawn.Should().BeFalse();
        }

        [Theory]
        [ClassData(typeof(SchemeStatusCoreData))]
        public void Map_GivenSourceWithNotWithdrawnSchemeAndComplianceYearIsNotClosed_CanSchemeManageEvidenceShouldBeTrue(SchemeStatus status)
        {
            if (status == SchemeStatus.Withdrawn)
            {
                return;
            }

            //arrange
            var currentDate = new DateTime(2020, 1, 1);
            var model = TestFixture.Build<ManageEvidenceNoteViewModel>()
                .With(m => m.SelectedComplianceYear, currentDate.Year).Create();

            var organisationId = TestFixture.Create<Guid>();
            var scheme = TestFixture.Build<SchemePublicInfo>().With(s => s.Status, status).Create();

            var transfer = new SchemeTabViewModelMapTransfer(organisationId,
                TestFixture.Create<EvidenceNoteSearchDataResult>(),
                scheme,
                currentDate,
                model,
                TestFixture.Create<int>());

            //act
            var result = viewAndTransferEvidenceViewModelMap.Map(transfer);

            //assert
            result.CanSchemeManageEvidence.Should().BeTrue();
        }

        [Fact]
        public void Map_GivenSourceWithWithdrawnSchemeAndComplianceYearIsNotClosed_CanSchemeManageEvidenceShouldBeFalse()
        {
            //arrange
            var currentDate = new DateTime(2020, 1, 1);
            var model = TestFixture.Build<ManageEvidenceNoteViewModel>()
                .With(m => m.SelectedComplianceYear, currentDate.Year).Create();

            var organisationId = TestFixture.Create<Guid>();
            var scheme = TestFixture.Build<SchemePublicInfo>().With(s => s.Status, SchemeStatus.Withdrawn).Create();

            var transfer = new SchemeTabViewModelMapTransfer(organisationId,
                TestFixture.Create<EvidenceNoteSearchDataResult>(),
                scheme,
                currentDate,
                model,
                TestFixture.Create<int>());

            //act
            var result = viewAndTransferEvidenceViewModelMap.Map(transfer);

            //assert
            result.CanSchemeManageEvidence.Should().BeFalse();
        }

        [Theory]
        [ClassData(typeof(OutOfComplianceYearData))]
        public void Map_GivenSourceWithNotWithdrawnSchemeAndComplianceYearIsClosed_CanSchemeManageEvidenceShouldBeFalse(DateTime currentDate, int complianceYear)
        {
            //arrange
            var model = TestFixture.Build<ManageEvidenceNoteViewModel>()
                .With(m => m.SelectedComplianceYear, complianceYear).Create();

            var organisationId = TestFixture.Create<Guid>();
            var scheme = TestFixture.Build<SchemePublicInfo>().With(s => s.Status, SchemeStatus.Approved).Create();

            var transfer = new SchemeTabViewModelMapTransfer(organisationId,
                TestFixture.Create<EvidenceNoteSearchDataResult>(),
                scheme,
                currentDate,
                model,
                TestFixture.Create<int>());

            //act
            var result = viewAndTransferEvidenceViewModelMap.Map(transfer);

            //assert
            result.CanSchemeManageEvidence.Should().BeFalse();
        }

        [Theory]
        [ClassData(typeof(OutOfComplianceYearData))]
        public void Map_GivenComplianceYearIsClosed_ComplianceYearClosedShouldBeTrue(DateTime currentDate, int complianceYear)
        {
            //arrange
            var model = TestFixture.Build<ManageEvidenceNoteViewModel>()
                .With(m => m.SelectedComplianceYear, complianceYear).Create();
            var organisationId = TestFixture.Create<Guid>();

            var transfer = new SchemeTabViewModelMapTransfer(organisationId,
                TestFixture.Create<EvidenceNoteSearchDataResult>(),
                TestFixture.Create<SchemePublicInfo>(),
                currentDate,
                model,
                TestFixture.Create<int>());

            //act
            var result = viewAndTransferEvidenceViewModelMap.Map(transfer);

            //assert
            result.ManageEvidenceNoteViewModel.ComplianceYearClosed.Should().BeTrue();
        }

        [Fact]
        public void Map_GivenComplianceYearIsNotClosed_ComplianceYearClosedShouldBeFalse()
        {
            //arrange
            var currentDate = new DateTime(2020, 1, 1);
            var model = TestFixture.Build<ManageEvidenceNoteViewModel>()
                .With(m => m.SelectedComplianceYear, currentDate.Year).Create();
            var organisationId = TestFixture.Create<Guid>();

            var transfer = new SchemeTabViewModelMapTransfer(organisationId,
                TestFixture.Create<EvidenceNoteSearchDataResult>(),
                TestFixture.Create<SchemePublicInfo>(),
                currentDate,
                model,
                TestFixture.Create<int>());

            //act
            var result = viewAndTransferEvidenceViewModelMap.Map(transfer);

            //assert
            result.ManageEvidenceNoteViewModel.ComplianceYearClosed.Should().BeFalse();
        }
    }
}
