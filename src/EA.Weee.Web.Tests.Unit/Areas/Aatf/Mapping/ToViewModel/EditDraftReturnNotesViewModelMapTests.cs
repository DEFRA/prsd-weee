﻿namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Mapping.ToViewModel
{
    using AutoFixture;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.Aatf.ViewModels;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Web.ViewModels.Shared;
    using Web.ViewModels.Shared.Mapping;
    using Weee.Tests.Core;
    using Weee.Tests.Core.DataHelpers;
    using Xunit;

    public class EditDraftReturnNotesViewModelMapTests : SimpleUnitTestBase
    {
        private readonly EditDraftReturnNotesViewModelMap editDraftReturnNotesViewModelMap;
        private readonly IMapper mapper;
        private readonly DateTime currentDate;

        public EditDraftReturnNotesViewModelMapTests()
        {
            mapper = A.Fake<IMapper>();

            editDraftReturnNotesViewModelMap = new EditDraftReturnNotesViewModelMap(mapper);
            currentDate = TestFixture.Create<DateTime>();
        }

        [Fact]
        public void EditDraftReturnNotesViewModelMap_ShouldBeDerivedFromListOfNotesViewModelBase()
        {
            typeof(EditDraftReturnNotesViewModelMap).Should()
                .BeDerivedFrom<ListOfNotesViewModelBase<EditDraftReturnedNotesViewModel>>();
        }

        [Fact]
        public void Map_GivenNullSource_ArgumentNulLExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => editDraftReturnNotesViewModelMap.Map(null));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenListOfEvidenceNotes_MapperShouldBeCalled()
        {
            //arrange
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();
            var model = TestFixture.Create<ManageEvidenceNoteViewModel>();
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var transfer = new EvidenceNotesViewModelTransfer(organisationId, aatfId, noteData, currentDate, model, 1, 2);

            //act
            editDraftReturnNotesViewModelMap.Map(transfer);

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
            var aatfId = Guid.NewGuid();
            var model = TestFixture.Create<ManageEvidenceNoteViewModel>();
            var transfer = new EvidenceNotesViewModelTransfer(organisationId, aatfId, noteData, currentDate, model, 1, 2);

            //act
            editDraftReturnNotesViewModelMap.Map(transfer);

            // assert 
            A.CallTo(() => mapper.Map<EvidenceNoteRowViewModel>(A<EvidenceNoteRowViewModel>._)).MustHaveHappened(0, Times.Exactly);
        }

        [Fact]
        public void Map_GivenAnEmptyListOfEvidenceNotes_MustReturnAnEmptyModel()
        {
            //arrange
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var model = TestFixture.Create<ManageEvidenceNoteViewModel>();
            var transfer = new EvidenceNotesViewModelTransfer(organisationId, aatfId, noteData, currentDate, model, 1, 2);

            //act
            var result = editDraftReturnNotesViewModelMap.Map(transfer);

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
            var model = TestFixture.Create<ManageEvidenceNoteViewModel>();
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var transfer = new EvidenceNotesViewModelTransfer(organisationId, aatfId, noteData, currentDate, model, 1, 2);
            A.CallTo(() => mapper.Map<List<EvidenceNoteRowViewModel>>(A<List<EvidenceNoteData>>._)).Returns(returnedNotes);

            //act
            var result = editDraftReturnNotesViewModelMap.Map(transfer);

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

            var model = TestFixture.Create<ManageEvidenceNoteViewModel>();
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var pageNumber = 1;
            var pageSize = 2;

            var transfer = new EvidenceNotesViewModelTransfer(organisationId, aatfId, noteData, currentDate, model, pageNumber, pageSize);

            A.CallTo(() => mapper.Map<List<EvidenceNoteRowViewModel>>(A<List<EvidenceNoteData>>._)).Returns(returnedNotes);

            //act
            var result = editDraftReturnNotesViewModelMap.Map(transfer);

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
            var date = new DateTime(2019, 1, 1);
            var transfer = new EvidenceNotesViewModelTransfer(TestFixture.Create<Guid>(), 
                TestFixture.Create<Guid>(), 
                noteData, date, model, 1, 2);

            //act
            var result = editDraftReturnNotesViewModelMap.Map(transfer);

            //assert
            result.ManageEvidenceNoteViewModel.ComplianceYearList.Count().Should().Be(3);
            result.ManageEvidenceNoteViewModel.ComplianceYearList.ElementAt(0).Should().Be(2019);
            result.ManageEvidenceNoteViewModel.ComplianceYearList.ElementAt(1).Should().Be(2018);
            result.ManageEvidenceNoteViewModel.ComplianceYearList.ElementAt(2).Should().Be(2017);
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
            var transfer = new EvidenceNotesViewModelTransfer(TestFixture.Create<Guid>(), TestFixture.Create<Guid>(), noteData, date, null, 1, 2);

            //act
            var result = editDraftReturnNotesViewModelMap.Map(transfer);

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
            var model = TestFixture.Build<ManageEvidenceNoteViewModel>()
                .With(m => m.SelectedComplianceYear, selectedComplianceYear).Create();
            var transfer = new EvidenceNotesViewModelTransfer(TestFixture.Create<Guid>(), 
                TestFixture.Create<Guid>(), 
                noteData, currentDate, model, 1, 2);

            //act
            var result = editDraftReturnNotesViewModelMap.Map(transfer);

            //assert
            result.ManageEvidenceNoteViewModel.SelectedComplianceYear.Should().Be(currentDate.Year);
        }

        [Fact]
        public void Map_GivenCurrentDateAndManageEvidenceViewModelWithSelectedComplianceYear_SelectedComplianceYearShouldBeSet()
        {
            //arrange
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();
            var model = TestFixture.Build<ManageEvidenceNoteViewModel>()
                .With(m => m.SelectedComplianceYear, currentDate.Year - 1).Create();
            var transfer = new EvidenceNotesViewModelTransfer(TestFixture.Create<Guid>(), 
                TestFixture.Create<Guid>(), noteData, TestFixture.Create<DateTime>(), model, 1, 2);

            //act
            var result = editDraftReturnNotesViewModelMap.Map(transfer);

            //assert
            result.ManageEvidenceNoteViewModel.SelectedComplianceYear.Should().Be(currentDate.Year - 1);
        }

        [Theory]
        [ClassData(typeof(OutOfComplianceYearData))]
        public void Map_GivenComplianceYearIsClosed_ComplianceYearClosedShouldBeTrue(DateTime currentDate, int complianceYear)
        {
            //arrange
            var model = TestFixture.Build<ManageEvidenceNoteViewModel>()
                .With(m => m.SelectedComplianceYear, complianceYear).Create();
            var organisationId = TestFixture.Create<Guid>();

            var transfer = new EvidenceNotesViewModelTransfer(organisationId,
                TestFixture.Create<Guid>(),
                TestFixture.Create<EvidenceNoteSearchDataResult>(),
                currentDate, model, 1, 2);

            //act
            var result = editDraftReturnNotesViewModelMap.Map(transfer);

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

            var transfer = new EvidenceNotesViewModelTransfer(organisationId,
                TestFixture.Create<Guid>(),
                TestFixture.Create<EvidenceNoteSearchDataResult>(),
                currentDate, model, 1, 2);

            //act
            var result = editDraftReturnNotesViewModelMap.Map(transfer);

            //assert
            result.ManageEvidenceNoteViewModel.ComplianceYearClosed.Should().BeFalse();
        }
    }
}
