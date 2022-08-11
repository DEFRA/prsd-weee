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

    public class TransferredOutEvidenceViewModelMapTests : SimpleUnitTestBase
    {
        private readonly TransferredOutEvidenceViewModelMap transferredOutEvidenceViewModelMap;
        private readonly IMapper mapper;
        private readonly ConfigurationService configurationService;

        public TransferredOutEvidenceViewModelMapTests()
        {
            mapper = A.Fake<IMapper>();
            configurationService = A.Fake<ConfigurationService>();
            A.CallTo(() => configurationService.CurrentConfiguration.DefaultPagingPageSize).Returns(25);

            transferredOutEvidenceViewModelMap = new TransferredOutEvidenceViewModelMap(mapper, configurationService);
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
            var result = transferredOutEvidenceViewModelMap.Map(transfer);

            //assert
            result.OrganisationId.Should().Be(organisationId);
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

            var transfer = new SchemeTabViewModelMapTransfer(organisationId,
                noteData,
                TestFixture.Create<SchemePublicInfo>(),
                TestFixture.Create<DateTime>(),
                TestFixture.Create<ManageEvidenceNoteViewModel>(),
                TestFixture.Create<int>());

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

            var transfer = new SchemeTabViewModelMapTransfer(organisationId,
                noteData,
                TestFixture.Create<SchemePublicInfo>(),
                TestFixture.Create<DateTime>(),
                TestFixture.Create<ManageEvidenceNoteViewModel>(),
                TestFixture.Create<int>());

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

            var transfer = new SchemeTabViewModelMapTransfer(organisationId,
                noteData,
                TestFixture.Create<SchemePublicInfo>(),
                TestFixture.Create<DateTime>(),
                TestFixture.Create<ManageEvidenceNoteViewModel>(),
                TestFixture.Create<int>());

            A.CallTo(() => mapper.Map<List<EvidenceNoteRowViewModel>>(A<List<EvidenceNoteData>>._)).Returns(returnedNotes);

            //act
            var result = transferredOutEvidenceViewModelMap.Map(transfer);

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

            A.CallTo(() => configurationService.CurrentConfiguration.DefaultPagingPageSize).Returns(pageSize);
            A.CallTo(() => mapper.Map<List<EvidenceNoteRowViewModel>>(A<List<EvidenceNoteData>>._)).Returns(returnedNotes);

            //act
            var result = transferredOutEvidenceViewModelMap.Map(transfer);

            // assert
            result.EvidenceNotesDataList.Should().NotBeEmpty();
            result.EvidenceNotesDataList.Should().BeEquivalentTo(returnedNotes);
            result.EvidenceNotesDataList.PageNumber.Should().Be(pageNumber);
            result.EvidenceNotesDataList.PageSize.Should().Be(pageSize);
        }

        [Fact]
        public void Map_GivenCurrentDate_ComplianceYearsListShouldBeReturned()
        {
            //arrange
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();
            var model = TestFixture.Create<ManageEvidenceNoteViewModel>();
            var date = new DateTime(2022, 1, 1);

            var source = new SchemeTabViewModelMapTransfer(TestFixture.Create<Guid>(),
                noteData,
                TestFixture.Create<SchemePublicInfo>(),
                date,
                model,
                TestFixture.Create<int>());

            //act
            var result = transferredOutEvidenceViewModelMap.Map(source);

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

            var source = new SchemeTabViewModelMapTransfer(TestFixture.Create<Guid>(),
                noteData,
                TestFixture.Create<SchemePublicInfo>(),
                date,
                null,
                TestFixture.Create<int>());

            //act
            var result = transferredOutEvidenceViewModelMap.Map(source);

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

            var source = new SchemeTabViewModelMapTransfer(TestFixture.Create<Guid>(),
                noteData,
                TestFixture.Create<SchemePublicInfo>(),
                date,
                model,
                TestFixture.Create<int>());

            //act
            var result = transferredOutEvidenceViewModelMap.Map(source);

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

            var source = new SchemeTabViewModelMapTransfer(TestFixture.Create<Guid>(),
                noteData,
                TestFixture.Create<SchemePublicInfo>(),
                date,
                model,
                TestFixture.Create<int>());

            //act
            var result = transferredOutEvidenceViewModelMap.Map(source);

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

            var transfer = new SchemeTabViewModelMapTransfer(organisationId,
                TestFixture.Create<EvidenceNoteSearchDataResult>(),
                scheme,
                TestFixture.Create<DateTime>(),
                TestFixture.Create<ManageEvidenceNoteViewModel>(),
                TestFixture.Create<int>());

            //act
            var result = transferredOutEvidenceViewModelMap.Map(transfer);

            //assert
            result.SchemeInfo.Should().Be(scheme);
        }

        [Fact]
        public void Map_GivenSourceWithWithdrawnScheme_ISWithdrawnShouldBeTrue()
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
            var result = transferredOutEvidenceViewModelMap.Map(transfer);

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
            var result = transferredOutEvidenceViewModelMap.Map(transfer);

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
            var result = transferredOutEvidenceViewModelMap.Map(transfer);

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
            var result = transferredOutEvidenceViewModelMap.Map(transfer);

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
            var result = transferredOutEvidenceViewModelMap.Map(transfer);

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
            var result = transferredOutEvidenceViewModelMap.Map(transfer);

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
            var result = transferredOutEvidenceViewModelMap.Map(transfer);

            //assert
            result.ManageEvidenceNoteViewModel.ComplianceYearClosed.Should().BeFalse();
        }
    }
}
