namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Mapping.ToViewModel
{
    using AutoFixture;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.AatfReturn;
    using Web.Areas.Aatf.Mappings.ToViewModel;
    using Weee.Tests.Core;
    using Weee.Tests.Core.SpecimenBuilders;
    using Xunit;

    public class AatfEvidenceSelectYourAatfViewModelMapTests : SimpleUnitTestBase
    {
        private readonly AatfEvidenceSelectYourAatfViewModelMap map;

        public AatfEvidenceSelectYourAatfViewModelMapTests()
        {
            map = new AatfEvidenceSelectYourAatfViewModelMap();

            TestFixture.Customizations.Add(new AatfFacilityTypeGenerator());
        }

        [Fact]
        public void Map_GivenNullSource_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => map.Map(null));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenSource_SelectYourAatfViewModelShouldBeReturned()
        {
            //act
            var result = map.Map(new AatfEvidenceToSelectYourAatfViewModelMapTransfer());

            //assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void Map_GivenSourceWithOrganisation_OrganisationShouldBeSet()
        {
            //arrange
            var organisationId = TestFixture.Create<Guid>();

            //act
            var result = map.Map(new AatfEvidenceToSelectYourAatfViewModelMapTransfer { OrganisationId = organisationId });

            //assert
            result.OrganisationId.Should().Be(organisationId);
        }

        [Fact]
        public void Map_GivenSourceWithEvidenceNoteStartDateThatIsAfterCurrentDate_EmptyListShouldBeReturned()
        {
            //arrange
            var aatfList = TestFixture.CreateMany<AatfData>().ToList();
            var currentDate = new DateTime(2020, 1, 1);
            var evidenceNoteStartDate = currentDate.AddDays(1);

            var source = TestFixture.Build<AatfEvidenceToSelectYourAatfViewModelMapTransfer>()
                .With(s => s.AatfList, aatfList)
                .With(s => s.CurrentDate, currentDate)
                .With(s => s.EvidenceSiteSelectionStartDateFrom, evidenceNoteStartDate)
                .Create();
            //act
            var result = map.Map(source);

            //assert
            result.AatfList.Should().BeEmpty();
        }

        public static IEnumerable<object[]> Dates =>
            new List<object[]>
            {
                new object[] { new DateTime(2020, 1, 31) },
                new object[] { new DateTime(2020, 2, 1) }
            };

        [Theory]
        [MemberData(nameof(Dates))]
        public void Map_GivenSourceWithEvidenceNoteStartDateThatIsBeforeOrEqualCurrentDate_AatfListShouldBeReturned(DateTime evidenceNoteStartDate)
        {
            //arrange
            var aatfList = TestFixture.Build<AatfData>().With(a => a.EvidenceSiteDisplay, true).CreateMany().ToList();
            var currentDate = new DateTime(2020, 2, 1);

            var source = TestFixture.Build<AatfEvidenceToSelectYourAatfViewModelMapTransfer>()
                .With(s => s.AatfList, aatfList)
                .With(s => s.CurrentDate, currentDate)
                .With(s => s.EvidenceSiteSelectionStartDateFrom, evidenceNoteStartDate)
                .Create();
            //act
            var result = map.Map(source);

            //assert
            result.AatfList.Should().BeEquivalentTo(aatfList);
        }

        [Fact]
        public void Map_GivenSourceWithDifferingDisplaySiteValues_ShouldReturnFilteredList()
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

            var source = TestFixture.Build<AatfEvidenceToSelectYourAatfViewModelMapTransfer>()
                .With(s => s.AatfList, aatfList)
                .With(s => s.CurrentDate, currentDate)
                .With(s => s.EvidenceSiteSelectionStartDateFrom, currentDate)
                .Create();

            //act
            var result = map.Map(source);

            //assert
            result.AatfList.Count.Should().Be(2);
            result.AatfList.Should().OnlyContain(a => a.EvidenceSiteDisplay);
        }

        [Fact]
        public void Map_GivenSourceWithAatfs_ShouldBeOrderedByAatfName()
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

            var source = TestFixture.Build<AatfEvidenceToSelectYourAatfViewModelMapTransfer>()
                .With(s => s.AatfList, aatfList)
                .With(s => s.CurrentDate, currentDate)
                .With(s => s.EvidenceSiteSelectionStartDateFrom, currentDate)
                .Create();

            //act
            var result = map.Map(source);

            //assert
            result.AatfList.Should().BeInAscendingOrder(a => a.Name);
        }

        [Fact]
        public void Map_GivenSourceWithMultipleSameAatfPerComplianceYears_ShouldReturnLatestAatf()
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

            var source = TestFixture.Build<AatfEvidenceToSelectYourAatfViewModelMapTransfer>()
                .With(s => s.AatfList, aatfList)
                .With(s => s.CurrentDate, currentDate)
                .With(s => s.EvidenceSiteSelectionStartDateFrom, currentDate)
                .Create();

            //act
            var result = map.Map(source);

            //assert
            result.AatfList.Count.Should().Be(2);
            result.AatfList.Should().Contain(a => a.ComplianceYear == 2020 && a.AatfId == aatfId2);
            result.AatfList.Should().Contain(a => a.ComplianceYear == 2019 && a.AatfId == aatfId1);
        }

        [Fact]
        public void Map_GivenSourceWithAatfs_ShouldHaveDisplayNameSet()
        {
            //arrange
            var aatfName1 = TestFixture.Create<string>();
            var aatfApprovalNumber1 = TestFixture.Create<string>();
            var aatfName2 = TestFixture.Create<string>();
            var aatfApprovalNumber2 = TestFixture.Create<string>();

            var aatfList = new List<AatfData>()
            {
                TestFixture.Build<AatfData>()
                    .With(a => a.EvidenceSiteDisplay, true)
                    .With(a => a.Name, aatfName1)
                    .With(a => a.ApprovalNumber, aatfApprovalNumber1)
                    .Create(),
                TestFixture.Build<AatfData>()
                    .With(a => a.EvidenceSiteDisplay, true)
                    .With(a => a.Name, aatfName2)
                    .With(a => a.ApprovalNumber, aatfApprovalNumber2)
                    .Create()
            };

            var currentDate = new DateTime(2020, 1, 1);

            var source = TestFixture.Build<AatfEvidenceToSelectYourAatfViewModelMapTransfer>()
                .With(s => s.AatfList, aatfList)
                .With(s => s.CurrentDate, currentDate)
                .With(s => s.EvidenceSiteSelectionStartDateFrom, currentDate)
                .Create();

            //act
            var result = map.Map(source);

            //assert
            result.AatfList.Count.Should().Be(2);
            result.AatfList.Should().Contain(a => a.Name == $"{aatfName1} ({aatfApprovalNumber1})");
            result.AatfList.Should().Contain(a => a.Name == $"{aatfName2} ({aatfApprovalNumber2})");
        }
    }
}
