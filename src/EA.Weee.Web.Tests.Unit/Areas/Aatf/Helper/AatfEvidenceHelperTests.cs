namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Helper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using Core.AatfReturn;
    using FakeItEasy;
    using FluentAssertions;
    using Services;
    using Web.Areas.Aatf.Helpers;
    using Weee.Tests.Core;
    using Xunit;

    public class AatfEvidenceHelperTests : SimpleUnitTestBase
    {
        private readonly AatfEvidenceHelper aatfHelper;
        private readonly ConfigurationService configurationService;

        public AatfEvidenceHelperTests()
        {
            configurationService = A.Fake<ConfigurationService>();
            aatfHelper = new AatfEvidenceHelper(configurationService);
        }

        [Fact]
        public void AatfCanEditCreateNotes_GivenAatfIsNotFound_ArgumentNullExceptionExpected()
        {
            //arrange
            var aatfs = TestFixture.CreateMany<AatfData>().ToList();

            //act
            var exception = Record.Exception(() =>
                aatfHelper.AatfCanEditCreateNotes(aatfs, TestFixture.Create<Guid>(), TestFixture.Create<int>()));
            
            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void AatfCanEditCreateNotes_GivenMatchingCriteria_TrueShouldBeReturned()
        {
            //arrange
            var id = TestFixture.Create<Guid>();
            var aatfId = TestFixture.Create<Guid>();
            var complianceYear = TestFixture.Create<int>();

            var aatf = TestFixture.Build<AatfData>()
                .With(a => a.Id, id)
                .With(a => a.AatfId, aatfId)
                .With(a => a.ComplianceYear, complianceYear)
                .With(a => a.CanCreateEditEvidence, true)
                .Create();

            var aatfs = new List<AatfData>()
            {
                TestFixture.Create<AatfData>(),
                aatf
            };

            //act
            var result = aatfHelper.AatfCanEditCreateNotes(aatfs, id, complianceYear);

            //assert
            result.Should().BeTrue();
        }

        [Fact]
        public void AatfCanEditCreateNotes_GivenMatchingCriteriaInAnotherComplianceYear_TrueShouldBeReturned()
        {
            //arrange
            var id = TestFixture.Create<Guid>();
            var aatfId = TestFixture.Create<Guid>();
            var complianceYear = TestFixture.Create<int>();

            var aatf1 = TestFixture.Build<AatfData>()
                .With(a => a.Id, id)
                .With(a => a.AatfId, aatfId)
                .With(a => a.ComplianceYear, complianceYear - 1)
                .With(a => a.CanCreateEditEvidence, false)
                .Create();

            var aatf2 = TestFixture.Build<AatfData>()
                .With(a => a.Id, id)
                .With(a => a.AatfId, aatfId)
                .With(a => a.ComplianceYear, complianceYear)
                .With(a => a.CanCreateEditEvidence, true)
                .Create();

            var aatfs = new List<AatfData>()
            {
                aatf1,
                aatf2
            };

            //act
            var result = aatfHelper.AatfCanEditCreateNotes(aatfs, id, complianceYear);

            //assert
            result.Should().BeTrue();
        }

        [Fact]
        public void AatfCanEditCreateNotes_GivenNonMatchingCriteriaInAnotherComplianceYear_FalseShouldBeReturned()
        {
            //arrange
            var id = TestFixture.Create<Guid>();
            var aatfId = TestFixture.Create<Guid>();
            var complianceYear = TestFixture.Create<int>();

            var aatf1 = TestFixture.Build<AatfData>()
                .With(a => a.Id, id)
                .With(a => a.AatfId, aatfId)
                .With(a => a.ComplianceYear, complianceYear - 1)
                .With(a => a.CanCreateEditEvidence, false)
                .Create();

            var aatf2 = TestFixture.Build<AatfData>()
                .With(a => a.Id, id)
                .With(a => a.AatfId, aatfId)
                .With(a => a.ComplianceYear, complianceYear)
                .With(a => a.CanCreateEditEvidence, false)
                .Create();

            var aatfs = new List<AatfData>()
            {
                aatf1,
                aatf2
            };

            //act
            var result = aatfHelper.AatfCanEditCreateNotes(aatfs, id, complianceYear);

            //assert
            result.Should().BeFalse();
        }

        [Fact]
        public void AatfCanEditCreateNotes_GivenNonMatchingComplianceYear_FalseShouldBeReturned()
        {
            //arrange
            var id = TestFixture.Create<Guid>();
            var aatfId = TestFixture.Create<Guid>();
            var complianceYear = TestFixture.Create<int>();

            var aatf = TestFixture.Build<AatfData>()
                .With(a => a.Id, id)
                .With(a => a.AatfId, aatfId)
                .With(a => a.ComplianceYear, complianceYear + 1)
                .With(a => a.CanCreateEditEvidence, true)
                .Create();

            var aatfs = new List<AatfData>()
            {
                TestFixture.Create<AatfData>(),
                aatf
            };

            //act
            var result = aatfHelper.AatfCanEditCreateNotes(aatfs, id, complianceYear);

            //assert
            result.Should().BeFalse();
        }

        [Fact]
        public void AatfCanEditCreateNotes_GivenNonMatchingCanCreateEditEvidence_FalseShouldBeReturned()
        {
            //arrange
            var id = TestFixture.Create<Guid>();
            var aatfId = TestFixture.Create<Guid>();
            var complianceYear = TestFixture.Create<int>();

            var aatf = TestFixture.Build<AatfData>()
                .With(a => a.Id, id)
                .With(a => a.AatfId, aatfId)
                .With(a => a.ComplianceYear, complianceYear + 1)
                .With(a => a.CanCreateEditEvidence, false)
                .Create();

            var aatfs = new List<AatfData>()
            {
                TestFixture.Create<AatfData>(),
                aatf
            };

            //act
            var result = aatfHelper.AatfCanEditCreateNotes(aatfs, id, complianceYear);

            //assert
            result.Should().BeFalse();
        }

        [Fact]
        public void GroupedValidAatfs_GivenSourceWithAatfsWithApprovalDateAfterEvidenceNoteBeforeDate_EmptyListShouldBeReturned()
        {
            //arrange
            var currentDate = new DateTime(2020, 1, 1);
            var aatfList = TestFixture.Build<AatfData>()
                .With(a => a.ApprovalDate, currentDate).CreateMany(3)
                .ToList();
            
            var evidenceNoteStartDate = currentDate.AddDays(1);
            A.CallTo(() => configurationService.CurrentConfiguration.EvidenceNotesSiteSelectionDateFrom)
                .Returns(evidenceNoteStartDate);

            //act
            var result = aatfHelper.GroupedValidAatfs(aatfList);

            //assert
            result.Should().BeEmpty();
        }

        public static IEnumerable<object[]> Dates =>
            new List<object[]>
            {
                new object[] { new DateTime(2020, 1, 31) },
                new object[] { new DateTime(2020, 2, 1) }
            };

        [Theory]
        [MemberData(nameof(Dates))]
        public void GroupedValidAatfs_GivenSourceWithEvidenceNoteStartDateThatIsBeforeOrEqualCurrentDate_AatfListShouldBeReturned(DateTime evidenceNoteStartDate)
        {
            //arrange
            var aatfId1 = TestFixture.Create<Guid>();
            var aatfId2 = TestFixture.Create<Guid>();
            var aatfId3 = TestFixture.Create<Guid>();
            var aatfId4 = TestFixture.Create<Guid>();

            var aatfList = new List<AatfData>()
            {
                TestFixture.Build<AatfData>()
                    .With(a => a.EvidenceSiteDisplay, true)
                    .With(a => a.ApprovalDate, evidenceNoteStartDate.AddDays(2))
                    .With(a => a.Id, aatfId1)
                    .Create(),
                TestFixture.Build<AatfData>()
                    .With(a => a.EvidenceSiteDisplay, true)
                    .With(a => a.ApprovalDate, evidenceNoteStartDate.AddDays(-1))
                    .With(a => a.Id, aatfId2)
                    .Create(),
                TestFixture.Build<AatfData>()
                    .With(a => a.EvidenceSiteDisplay, true)
                    .With(a => a.ApprovalDate, evidenceNoteStartDate)
                    .With(a => a.Id, aatfId4)
                    .Create(),
                TestFixture.Build<AatfData>()
                    .With(a => a.EvidenceSiteDisplay, true)
                    .With(a => a.ApprovalDate, evidenceNoteStartDate.AddDays(1))
                    .With(a => a.Id, aatfId3)
                    .Create()
            };

            A.CallTo(() => configurationService.CurrentConfiguration.EvidenceNotesSiteSelectionDateFrom)
                .Returns(evidenceNoteStartDate);

            //act
            var result = aatfHelper.GroupedValidAatfs(aatfList);

            //assert
            result.Count.Should().Be(2);
            result.FirstOrDefault(a => a.Id == aatfId1).Should().NotBeNull();
            result.FirstOrDefault(a => a.Id == aatfId3).Should().NotBeNull();
        }

        [Fact]
        public void GroupedValidAatfs_GivenSourceWithDifferingDisplaySiteValues_ShouldReturnFilteredList()
        {
            //arrange
            var aatfList = new List<AatfData>()
            {
                TestFixture.Build<AatfData>().With(a => a.EvidenceSiteDisplay, false).Create(),
                TestFixture.Build<AatfData>().With(a => a.EvidenceSiteDisplay, true).Create(),
                TestFixture.Build<AatfData>().With(a => a.EvidenceSiteDisplay, true).Create(),
                TestFixture.Build<AatfData>().With(a => a.EvidenceSiteDisplay, false).Create()
            };

            var currentDate = new DateTime(2020, 1, 1);
            A.CallTo(() => configurationService.CurrentConfiguration.EvidenceNotesSiteSelectionDateFrom)
                .Returns(currentDate);

            //act
            var result = aatfHelper.GroupedValidAatfs(aatfList);

            //assert
            result.Count.Should().Be(2);
            result.Should().OnlyContain(a => a.EvidenceSiteDisplay);
        }

        [Fact]
        public void GroupedValidAatfs_GivenSourceWithAatfs_ShouldBeOrderedByAatfName()
        {
            //arrange
            var aatfList = new List<AatfData>()
            {
                TestFixture.Build<AatfData>()
                    .With(a => a.EvidenceSiteDisplay, true)
                    .With(a => a.Name, "C")
                    .Create(),
                TestFixture.Build<AatfData>()
                    .With(a => a.EvidenceSiteDisplay, true)
                    .With(a => a.Name, "Z")
                    .Create(),
                TestFixture.Build<AatfData>()
                    .With(a => a.EvidenceSiteDisplay, true)
                    .With(a => a.Name, "A").Create()
            };

            var currentDate = new DateTime(2020, 1, 1);
            A.CallTo(() => configurationService.CurrentConfiguration.EvidenceNotesSiteSelectionDateFrom)
                .Returns(currentDate);

            //act
            var result = aatfHelper.GroupedValidAatfs(aatfList);

            //assert
            result.Should().BeInAscendingOrder(a => a.Name);
        }

        [Fact]
        public void GroupedValidAatfs_GivenSourceWithMultipleSameAatfPerComplianceYears_ShouldReturnLatestAatf()
        {
            //arrange
            var aatfId1 = TestFixture.Create<Guid>();
            var aatfId2 = TestFixture.Create<Guid>();

            var aatfList = new List<AatfData>()
            {
                TestFixture.Build<AatfData>()
                    .With(a => a.EvidenceSiteDisplay, true)
                    .With(a => a.AatfId, aatfId1)
                    .With(a => a.ComplianceYear, 2018)
                    .Create(),
                TestFixture.Build<AatfData>()
                    .With(a => a.EvidenceSiteDisplay, true)
                    .With(a => a.AatfId, aatfId2)
                    .With(a => a.ComplianceYear, 2019)
                    .Create(),
                TestFixture.Build<AatfData>()
                    .With(a => a.EvidenceSiteDisplay, true)
                    .With(a => a.AatfId, aatfId2)
                    .With(a => a.ComplianceYear, 2020)
                    .Create(),
                TestFixture.Build<AatfData>()
                    .With(a => a.EvidenceSiteDisplay, true)
                    .With(a => a.AatfId, aatfId2)
                    .With(a => a.ComplianceYear, 2019)
                    .Create(),
                TestFixture.Build<AatfData>()
                    .With(a => a.EvidenceSiteDisplay, true)
                    .With(a => a.ComplianceYear, 2019)
                    .With(a => a.AatfId, aatfId1).Create()
            };

            var currentDate = new DateTime(2020, 1, 1);
            A.CallTo(() => configurationService.CurrentConfiguration.EvidenceNotesSiteSelectionDateFrom)
                .Returns(currentDate);

            //act
            var result = aatfHelper.GroupedValidAatfs(aatfList);

            //assert
            result.Count.Should().Be(2);
            result.Should().Contain(a => a.ComplianceYear == 2020 && a.AatfId == aatfId2);
            result.Should().Contain(a => a.ComplianceYear == 2019 && a.AatfId == aatfId1);
        }
    }
}
