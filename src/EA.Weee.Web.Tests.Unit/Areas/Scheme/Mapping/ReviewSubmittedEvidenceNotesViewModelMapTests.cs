﻿namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Mapping
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
    using Web.ViewModels.Shared;
    using Web.ViewModels.Shared.Mapping;
    using Weee.Tests.Core;
    using Weee.Tests.Core.DataHelpers;
    using Xunit;

    public class ReviewSubmittedEvidenceNotesViewModelMapTests : SimpleUnitTestBase
    {
        private readonly ReviewSubmittedEvidenceNotesViewModelMap reviewSubmittedEvidenceNotesViewModelMap;
        private readonly IMapper mapper;

        public ReviewSubmittedEvidenceNotesViewModelMapTests()
        {
            mapper = A.Fake<IMapper>();

            reviewSubmittedEvidenceNotesViewModelMap = new ReviewSubmittedEvidenceNotesViewModelMap(mapper);
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
            var exception = Record.Exception(() => new SchemeTabViewModelMapTransfer(Guid.NewGuid(),
                null, TestFixture.Create<SchemePublicInfo>(), TestFixture.Create<DateTime>(),
                TestFixture.Create<int>(),
                1, 2));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenOrganisationGuidIsEmpty_ArgumentExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => new SchemeTabViewModelMapTransfer(Guid.Empty,
                TestFixture.Create<EvidenceNoteSearchDataResult>(), TestFixture.Create<SchemePublicInfo>(),
                TestFixture.Create<DateTime>(), TestFixture.Create<int>(), 
                1, 2));

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
                TestFixture.Create<int>(),
                1, 2);

            //act
            reviewSubmittedEvidenceNotesViewModelMap.Map(transfer);

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
                TestFixture.Create<int>(),
                1, 2);

            //act
            reviewSubmittedEvidenceNotesViewModelMap.Map(transfer);

            // assert 
            A.CallTo(() => mapper.Map<EvidenceNoteRowViewModel>(A<EvidenceNoteRowViewModel>._))
                .MustHaveHappened(0, Times.Exactly);
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
                TestFixture.Create<int>(),
                1, 2);

            //act
            var result = reviewSubmittedEvidenceNotesViewModelMap.Map(transfer);

            // assert 
            result.EvidenceNotesDataList.Should().BeNullOrEmpty();
        }

        [Fact]
        public void Map_GivenListOfEvidenceNoteData_ShouldReturnMappedData()
        {
            //arrange
            var returnedNotes = new List<EvidenceNoteRowViewModel>
            {
                TestFixture.Create<EvidenceNoteRowViewModel>(),
                TestFixture.Create<EvidenceNoteRowViewModel>(),
                TestFixture.Create<EvidenceNoteRowViewModel>()
            };

            var notes = TestFixture.Create<EvidenceNoteSearchDataResult>();

            var organisationId = Guid.NewGuid();

            var transfer = new SchemeTabViewModelMapTransfer(organisationId,
                notes,
                TestFixture.Create<SchemePublicInfo>(),
                TestFixture.Create<DateTime>(),
                TestFixture.Create<int>(), 1, 3);

            A.CallTo(() => mapper.Map<List<EvidenceNoteRowViewModel>>(A<List<EvidenceNoteData>>._))
                .Returns(returnedNotes);

            //act
            var result = reviewSubmittedEvidenceNotesViewModelMap.Map(transfer);

            // assert
            result.EvidenceNotesDataList.Should().NotBeEmpty();
            result.EvidenceNotesDataList.Should().BeEquivalentTo(returnedNotes);
        }

        [Fact]
        public void Map_GivenListOfEvidenceNoteData_ShouldReturnMappedDataAsPagedList()
        {
            //arrange
            var returnedNotes = new List<EvidenceNoteRowViewModel>
            {
                TestFixture.Create<EvidenceNoteRowViewModel>(),
                TestFixture.Create<EvidenceNoteRowViewModel>(),
                TestFixture.Create<EvidenceNoteRowViewModel>()
            };

            var notes = TestFixture.Create<EvidenceNoteSearchDataResult>();
            var organisationId = Guid.NewGuid();
            var pageNumber = 1;
            var pageSize = 3;

            var transfer = new SchemeTabViewModelMapTransfer(organisationId,
                notes,
                TestFixture.Create<SchemePublicInfo>(),
                TestFixture.Create<DateTime>(),
                TestFixture.Create<int>(), pageNumber, pageSize);

            A.CallTo(() => mapper.Map<List<EvidenceNoteRowViewModel>>(A<List<EvidenceNoteData>>._)).Returns(returnedNotes);

            //act
            var result = reviewSubmittedEvidenceNotesViewModelMap.Map(transfer);

            // assert
            result.EvidenceNotesDataList.Should().NotBeEmpty();
            result.EvidenceNotesDataList.Should().BeEquivalentTo(returnedNotes);
            result.EvidenceNotesDataList.PageNumber.Should().Be(pageNumber);
            result.EvidenceNotesDataList.PageSize.Should().Be(pageSize);
        }

        [Fact]
        public void Map_GivenSource_PropertiesShouldBeSet()
        {
            //arrange
            var organisationId = TestFixture.Create<Guid>();
            var scheme = TestFixture.Create<SchemePublicInfo>();
            var transfer = new SchemeTabViewModelMapTransfer(organisationId,
                TestFixture.Create<EvidenceNoteSearchDataResult>(),
                scheme,
                TestFixture.Create<DateTime>(),
                TestFixture.Create<int>(), 
                1, 2);

            //act
            var result = reviewSubmittedEvidenceNotesViewModelMap.Map(transfer);

            //assert
            result.OrganisationId.Should().Be(organisationId);
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
                TestFixture.Create<int>(), 
                1, 2);

            //act
            var result = reviewSubmittedEvidenceNotesViewModelMap.Map(transfer);

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
                TestFixture.Create<int>(), 
                1, 2);

            //act
            var result = reviewSubmittedEvidenceNotesViewModelMap.Map(transfer);

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
                TestFixture.Create<int>(), 
                1, 2);

            //act
            var result = reviewSubmittedEvidenceNotesViewModelMap.Map(transfer);

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
            var organisationId = TestFixture.Create<Guid>();
            var scheme = TestFixture.Build<SchemePublicInfo>().With(s => s.Status, status).Create();

            var transfer = new SchemeTabViewModelMapTransfer(organisationId,
                TestFixture.Create<EvidenceNoteSearchDataResult>(),
                scheme,
                currentDate,
                currentDate.Year,
                1, 2);

            //act
            var result = reviewSubmittedEvidenceNotesViewModelMap.Map(transfer);

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
                currentDate.Year, 
                1, 2);

            //act
            var result = reviewSubmittedEvidenceNotesViewModelMap.Map(transfer);

            //assert
            result.CanSchemeManageEvidence.Should().BeFalse();
        }

        [Fact]
        public void Map_GivenComplianceYearIsNotClosed_ComplianceYearClosedShouldBeFalse()
        {
            //arrange
            var currentDate = new DateTime(2020, 1, 1);
            var organisationId = TestFixture.Create<Guid>();

            var transfer = new SchemeTabViewModelMapTransfer(organisationId,
                TestFixture.Create<EvidenceNoteSearchDataResult>(),
                TestFixture.Create<SchemePublicInfo>(),
                currentDate,
                currentDate.Year,
                1, 2);

            //act
            var result = reviewSubmittedEvidenceNotesViewModelMap.Map(transfer);

            //assert
            result.ManageEvidenceNoteViewModel.ComplianceYearClosed.Should().BeFalse();
        }

        [Theory]
        [ClassData(typeof(OutOfComplianceYearData))]
        public void Map_GivenSourceWithNotWithdrawnSchemeAndComplianceYearIsClosed_CanSchemeManageEvidenceShouldBeFalse(DateTime currentDate, int complianceYear)
        {
            //arrange
            var organisationId = TestFixture.Create<Guid>();
            var scheme = TestFixture.Build<SchemePublicInfo>().With(s => s.Status, SchemeStatus.Approved).Create();

            var transfer = new SchemeTabViewModelMapTransfer(organisationId,
                TestFixture.Create<EvidenceNoteSearchDataResult>(),
                scheme,
                currentDate,
                complianceYear,
                1, 2);

            //act
            var result = reviewSubmittedEvidenceNotesViewModelMap.Map(transfer);

            //assert
            result.CanSchemeManageEvidence.Should().BeFalse();
        }
    }
}
