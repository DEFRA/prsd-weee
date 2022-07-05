﻿namespace EA.Weee.RequestHandlers.Tests.Unit.Mapping
{
    using AutoFixture;
    using Core.AatfReturn;
    using Domain.AatfReturn;
    using FakeItEasy;
    using FluentAssertions;
    using Mappings;
    using Prsd.Core.Mapper;
    using System;
    using System.Collections.Generic;
    using Weee.Tests.Core;
    using Xunit;
    using FacilityType = Core.AatfReturn.FacilityType;

    public class AatfDataWithSystemTimeMapTests : SimpleUnitTestBase
    {
        private readonly IMap<Aatf, AatfData> aatfMap;
        private readonly AatfDataWithSystemTimeMap map;

        public AatfDataWithSystemTimeMapTests()
        {
            aatfMap = A.Fake<IMap<Aatf, AatfData>>();

            map = new AatfDataWithSystemTimeMap(aatfMap);
        }

        [Fact]
        public void Map_GivenNullSource_ShouldThrowArgumentNullException()
        {
            //act
            var exception = Record.Exception(() => map.Map(null));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenSourceAatf_AatfDataShouldBeMapped()
        {
            //arrange
            var aatf = TestFixture.Create<Aatf>();

            //act
            map.Map(new AatfWithSystemDateMapperObject(aatf, TestFixture.Create<DateTime>()));

            //assert
            A.CallTo(() => aatfMap.Map(aatf)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Map_GivenMappedAatfData_AatfDataShouldBeReturned()
        {
            //arrange
            var aatfData = TestFixture.Create<AatfData>();
            A.CallTo(() => aatfMap.Map(A<Aatf>._)).Returns(aatfData);

            //act
            var result = map.Map(new AatfWithSystemDateMapperObject(TestFixture.Create<Aatf>(), TestFixture.Create<DateTime>()));

            //assert
            result.Should().Be(aatfData);
        }

        [Fact]
        public void Map_GivenMappedAatfDataHasEvidenceNotesAndHasValidApprovalDate_EvidenceSiteDisplayShouldBeTrue()
        {
            //arrange
            var approvalDate = new DateTime(2020, 1, 1);
            var date = approvalDate.AddDays(1);
            var aatfData = TestFixture.Build<AatfData>()
                .With(a => a.HasEvidenceNotes, false)
                .With(a => a.FacilityType, FacilityType.Aatf)
                .With(a => a.ApprovalDate, approvalDate)
                .Create();
            A.CallTo(() => aatfMap.Map(A<Aatf>._)).Returns(aatfData);

            //act
            var result = map.Map(new AatfWithSystemDateMapperObject(TestFixture.Create<Aatf>(), date));

            //assert
            result.EvidenceSiteDisplay.Should().BeTrue();
        }

        [Fact]
        public void Map_GivenMappedAatfDataHasEvidenceNotesAndHasValidApprovalDateButIsAnAe_EvidenceSiteDisplayShouldBeFalse()
        {
            //arrange
            var approvalDate = new DateTime(2020, 1, 1);
            var date = approvalDate.AddDays(1);
            var aatfData = TestFixture.Build<AatfData>()
                .With(a => a.HasEvidenceNotes, false)
                .With(a => a.FacilityType, FacilityType.Ae)
                .With(a => a.ApprovalDate, approvalDate)
                .Create();
            A.CallTo(() => aatfMap.Map(A<Aatf>._)).Returns(aatfData);

            //act
            var result = map.Map(new AatfWithSystemDateMapperObject(TestFixture.Create<Aatf>(), date));

            //assert
            result.EvidenceSiteDisplay.Should().BeFalse();
        }

        [Fact]
        public void Map_GivenMappedAatfDataHasEvidenceNotesAndHasInValidApprovalDate_EvidenceSiteDisplayShouldBeFalse()
        {
            //arrange
            var approvalDate = new DateTime(2020, 1, 1);
            var date = approvalDate.AddDays(-1);
            var aatfData = TestFixture.Build<AatfData>()
                .With(a => a.HasEvidenceNotes, false)
                .With(a => a.FacilityType, FacilityType.Aatf)
                .With(a => a.ApprovalDate, approvalDate)
                .Create();
            A.CallTo(() => aatfMap.Map(A<Aatf>._)).Returns(aatfData);

            //act
            var result = map.Map(new AatfWithSystemDateMapperObject(TestFixture.Create<Aatf>(), date));

            //assert
            result.EvidenceSiteDisplay.Should().BeFalse();
        }

        [Fact]
        public void Map_GivenMappedAatfDataHasNoEvidenceNotesAndNoApprovalDate_EvidenceSiteDisplayShouldBeTrue()
        {
            //arrange
            var aatfData = TestFixture.Build<AatfData>()
                .With(a => a.HasEvidenceNotes, false)
                .With(a => a.FacilityType, FacilityType.Aatf)
                .With(a => a.ApprovalDate, (DateTime?)null)
                .Create();

            A.CallTo(() => aatfMap.Map(A<Aatf>._)).Returns(aatfData);

            //act
            var result = map.Map(new AatfWithSystemDateMapperObject(TestFixture.Create<Aatf>(), TestFixture.Create<DateTime>()));

            //assert
            result.EvidenceSiteDisplay.Should().BeFalse();
        }

        [Fact]
        public void Map_GivenMappedAatfDataHasNoEvidenceNotesAndApprovalDateOutsideOfComplianceYearEndDate_EvidenceSiteDisplayShouldBeFalse()
        {
            //arrange
            var date = new DateTime(2020, 2, 1);
            var approvalDate = new DateTime(2021, 2, 1);
            var aatfData = TestFixture.Build<AatfData>()
                .With(a => a.HasEvidenceNotes, false)
                .With(a => a.FacilityType, FacilityType.Aatf)
                .With(a => a.ApprovalDate, approvalDate)
                .Create();

            A.CallTo(() => aatfMap.Map(A<Aatf>._)).Returns(aatfData);

            //act
            var result = map.Map(new AatfWithSystemDateMapperObject(TestFixture.Create<Aatf>(), date));

            //assert
            result.EvidenceSiteDisplay.Should().BeFalse();
        }

        [Fact]
        public void Map_GivenMappedAatfDataHasNoEvidenceNotesAndApprovalDateOutsideOfCurrentDate_EvidenceSiteDisplayShouldBeFalse()
        {
            //arrange
            var date = new DateTime(2020, 2, 1);
            var approvalDate = new DateTime(2020, 2, 2);
            var aatfData = TestFixture.Build<AatfData>()
                .With(a => a.HasEvidenceNotes, false)
                .With(a => a.FacilityType, FacilityType.Aatf)
                .With(a => a.ApprovalDate, approvalDate)
                .Create();

            A.CallTo(() => aatfMap.Map(A<Aatf>._)).Returns(aatfData);

            //act
            var result = map.Map(new AatfWithSystemDateMapperObject(TestFixture.Create<Aatf>(), date));

            //assert
            result.EvidenceSiteDisplay.Should().BeFalse();
        }

        public static IEnumerable<object[]> Dates =>
            new List<object[]>
            {
                new object[] { new DateTime(2020, 5, 5), new DateTime(2020, 5, 4) },
                new object[] { new DateTime(2020, 5, 5), new DateTime(2020, 5, 5) },
            };

        [Theory]
        [MemberData(nameof(Dates))]

        public void Map_GivenMappedAatfDataHasNoEvidenceNotesAndApprovalDateIsValid_EvidenceSiteDisplayShouldBeTrue(DateTime currentDate, DateTime approvalDate)
        {
            //arrange
            var aatfData = TestFixture.Build<AatfData>()
                .With(a => a.HasEvidenceNotes, false)
                .With(a => a.FacilityType, FacilityType.Aatf)
                .With(a => a.ApprovalDate, approvalDate)
                .Create();

            A.CallTo(() => aatfMap.Map(A<Aatf>._)).Returns(aatfData);

            //act
            var result = map.Map(new AatfWithSystemDateMapperObject(TestFixture.Create<Aatf>(), currentDate));

            //assert
            result.EvidenceSiteDisplay.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(Dates))]

        public void Map_GivenMappedAatfDataHasNoEvidenceNotesAndApprovalDateIsValidButFacilityTypeIsAe_EvidenceSiteDisplayShouldBeFalse(DateTime currentDate, DateTime approvalDate)
        {
            //arrange
            var aatfData = TestFixture.Build<AatfData>()
                .With(a => a.HasEvidenceNotes, false)
                .With(a => a.FacilityType, FacilityType.Ae)
                .With(a => a.ApprovalDate, approvalDate)
                .Create();

            A.CallTo(() => aatfMap.Map(A<Aatf>._)).Returns(aatfData);

            //act
            var result = map.Map(new AatfWithSystemDateMapperObject(TestFixture.Create<Aatf>(), currentDate));

            //assert
            result.EvidenceSiteDisplay.Should().BeFalse();
        }
    }
}