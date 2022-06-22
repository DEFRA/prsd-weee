namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Mapping
{
    using AutoFixture;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Web.Areas.Aatf.ViewModels;
    using EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels;
    using EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Web.ViewModels.Shared;
    using Web.ViewModels.Shared.Mapping;
    using Xunit;

    public class ReviewSubmittedEvidenceNotesViewModelMapTests
    {
        private readonly ReviewSubmittedEvidenceNotesViewModelMap reviewSubmittedEvidenceNotesViewModelMap;
        private readonly Fixture fixture;
        private readonly IMapper mapper;

        public ReviewSubmittedEvidenceNotesViewModelMapTests()
        {
            mapper = A.Fake<IMapper>();

            reviewSubmittedEvidenceNotesViewModelMap = new ReviewSubmittedEvidenceNotesViewModelMap(mapper);

            fixture = new Fixture();
        }

        [Fact]
        public void ReviewSubmittedEvidenceNotesViewModelMap_ShouldBeDerivedFromListOfNotesViewModelBase()
        {
            typeof(ReviewSubmittedEvidenceNotesViewModelMap).Should()
                .BeDerivedFrom<ListOfNotesViewModelBase<ReviewSubmittedManageEvidenceNotesSchemeViewModel>>();
        }

        [Fact]
        public void Map_GiveListOfNotesIsNull_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => new ReviewSubmittedEvidenceNotesViewModelMapTransfer(Guid.NewGuid(), null, "Test", fixture.Create<DateTime>(), fixture.Create<ManageEvidenceNoteViewModel>()));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenOrganisationGuidIsEmpty_ArgumentExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => new ReviewSubmittedEvidenceNotesViewModelMapTransfer(Guid.Empty, fixture.CreateMany<EvidenceNoteData>().ToList(), "Test", fixture.Create<DateTime>(), fixture.Create<ManageEvidenceNoteViewModel>()));

            //assert
            exception.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void Map_GivenSchemeNameIsNull_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => new ReviewSubmittedEvidenceNotesViewModelMapTransfer(Guid.NewGuid(), fixture.CreateMany<EvidenceNoteData>().ToList(), null, fixture.Create<DateTime>(), fixture.Create<ManageEvidenceNoteViewModel>()));

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

            var transfer = new ReviewSubmittedEvidenceNotesViewModelMapTransfer(organisationId, 
                notes, 
                "Test", 
                fixture.Create<DateTime>(),
                fixture.Create<ManageEvidenceNoteViewModel>());

            //act
            reviewSubmittedEvidenceNotesViewModelMap.Map(transfer);

            // assert 
            A.CallTo(() => mapper.Map<List<EvidenceNoteRowViewModel>>(notes)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Map_GivenAnEmptyListOfEvidenceNotes_MapperShouldNotBeCalled()
        {
            //arrange
            var notes = new List<EvidenceNoteData>();

            var organisationId = Guid.NewGuid();

            var transfer = new ReviewSubmittedEvidenceNotesViewModelMapTransfer(organisationId, 
                notes, 
                "Test", 
                fixture.Create<DateTime>(),
                fixture.Create<ManageEvidenceNoteViewModel>());

            //act
            reviewSubmittedEvidenceNotesViewModelMap.Map(transfer);

            // assert 
            A.CallTo(() => mapper.Map<EvidenceNoteRowViewModel>(A<EvidenceNoteRowViewModel>._)).MustHaveHappened(0, Times.Exactly);
        }

        [Fact]
        public void Map_GivenAnEmptyListOfEvidenceNotes_MustReturnAnEmptyModel()
        {
            //arrange
            var notes = new List<EvidenceNoteData>();

            var organisationId = Guid.NewGuid();

            var transfer = new ReviewSubmittedEvidenceNotesViewModelMapTransfer(organisationId, 
                notes, 
                "Test", 
                fixture.Create<DateTime>(),
                fixture.Create<ManageEvidenceNoteViewModel>());

            //act
            var result = reviewSubmittedEvidenceNotesViewModelMap.Map(transfer);

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

            var organisationId = Guid.NewGuid();

            var transfer = new ReviewSubmittedEvidenceNotesViewModelMapTransfer(organisationId, 
                notes, 
                "Test", 
                fixture.Create<DateTime>(),
                fixture.Create<ManageEvidenceNoteViewModel>());

            A.CallTo(() => mapper.Map<List<EvidenceNoteRowViewModel>>(A<List<EvidenceNoteData>>._)).Returns(returnedNotes);
            
            //act
            var result = reviewSubmittedEvidenceNotesViewModelMap.Map(transfer);

            // assert
            result.EvidenceNotesDataList.Should().NotBeEmpty();
            result.EvidenceNotesDataList.Should().BeEquivalentTo(returnedNotes);
        }

        [Fact]
        public void Map_GivenSchemeNameAndOrganisationId_PropertiesShouldBeSet()
        {
            //arrange
            var organisationId = fixture.Create<Guid>();
            var schemeName = fixture.Create<string>();

            var transfer = new ReviewSubmittedEvidenceNotesViewModelMapTransfer(organisationId,
                fixture.CreateMany<EvidenceNoteData>().ToList(),
                schemeName,
                fixture.Create<DateTime>(),
                fixture.Create<ManageEvidenceNoteViewModel>());

            //act
            var result = reviewSubmittedEvidenceNotesViewModelMap.Map(transfer);

            //assert
            result.OrganisationId.Should().Be(organisationId);
            result.SchemeName.Should().Be(schemeName);
        }

        [Fact]
        public void Map_GivenCurrentDate_ComplianceYearsListShouldBeReturned()
        {
            //arrange
            var notes = fixture.CreateMany<EvidenceNoteData>().ToList();
            var model = fixture.Create<ManageEvidenceNoteViewModel>();
            var date = new DateTime(2022, 1, 1);

            //act
            var result = reviewSubmittedEvidenceNotesViewModelMap.MapBase(notes, date, model);

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
            var result = reviewSubmittedEvidenceNotesViewModelMap.MapBase(notes, date, null);

            //assert
            result.ManageEvidenceNoteViewModel.SelectedComplianceYear.Should().Be(year);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Map_GivenCurrentDateAndManageEvidenceViewModelSelectedComplianceYearIsNotGreaterThanZero_SelectedComplianceYearShouldBeSet(int selectedComplianceYear)
        {
            //arrange
            var notes = fixture.CreateMany<EvidenceNoteData>().ToList();
            var date = new DateTime(2022, 1, 1);
            var model = fixture.Build<ManageEvidenceNoteViewModel>()
                .With(m => m.SelectedComplianceYear, selectedComplianceYear).Create();

            //act
            var result = reviewSubmittedEvidenceNotesViewModelMap.MapBase(notes, date, model);

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
            var result = reviewSubmittedEvidenceNotesViewModelMap.MapBase(notes, date, model);

            //assert
            result.ManageEvidenceNoteViewModel.SelectedComplianceYear.Should().Be(2021);
        }
    }
}
