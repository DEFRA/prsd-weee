namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Mapping.Filters
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.Aatf.Mappings.Filters;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class AatfDataFilterTests
    {
        private readonly AatfDataAatfDataFilter aatfDataAatfDataFilter;

        public AatfDataFilterTests()
        {
            aatfDataAatfDataFilter = new AatfDataAatfDataFilter();
        }

        [Fact]
        public void Filter_GivenListOfAatfs_ContactDetailsNameIsBuilt()
        {
            var aatfList = new List<AatfData>();
            var facilityType = FacilityType.Aatf;

            var aatfData = new AatfData(Guid.NewGuid(), "AATF", "approval number", 2019, A.Dummy<Core.Shared.UKCompetentAuthorityData>(),
                   Core.AatfReturn.AatfStatus.Approved, A.Dummy<AatfAddressData>(), Core.AatfReturn.AatfSize.Large, DateTime.Now,
                   A.Dummy<Core.Shared.PanAreaData>(), null)
            {
                FacilityType = facilityType,
            };

            aatfList.Add(aatfData);

            var result = aatfDataAatfDataFilter.Filter(aatfList, facilityType, true);

            foreach (var aatf in result)
            {
                aatf.AatfContactDetailsName.Should().Be(aatf.Name + " (" + aatf.ApprovalNumber + ") - " + aatf.AatfStatus);
            }
        }

        [Fact]
        public void Filter_GivenListOfAatfs_GivenAatfsWithTheSameAatfIdTheLatestApprovalDateShouldBeReturned()
        {
            var aatfList = new List<AatfData>();
            var aatfId = Guid.NewGuid();
            var facilityType = FacilityType.Aatf;

            var aatfData = new AatfData(Guid.NewGuid(), "AATF", "approval number", 2019, A.Dummy<Core.Shared.UKCompetentAuthorityData>(),
                   Core.AatfReturn.AatfStatus.Approved, A.Dummy<AatfAddressData>(), Core.AatfReturn.AatfSize.Large, new DateTime(2018, 10, 1),
                   A.Dummy<Core.Shared.PanAreaData>(), null)
            {
                FacilityType = facilityType,
                AatfId = aatfId
            };

            var aatfData2 = new AatfData(Guid.NewGuid(), "AATF 2020", "approval number", 2020, A.Dummy<Core.Shared.UKCompetentAuthorityData>(),
               Core.AatfReturn.AatfStatus.Approved, A.Dummy<AatfAddressData>(), Core.AatfReturn.AatfSize.Large, new DateTime(2019, 10, 1),
               A.Dummy<Core.Shared.PanAreaData>(), null)
            {
                FacilityType = facilityType,
                AatfId = aatfId
            };

            aatfList.Add(aatfData);
            aatfList.Add(aatfData2);

            var result = aatfDataAatfDataFilter.Filter(aatfList, facilityType, true);

            result.Count.Should().Be(1);
            result.Should().Contain(aatfData2);
            result.Should().NotContain(aatfData);
        }

        [Fact]
        public void Filter_GivenAatfsAndAatfFacilityType_AatfListShouldOnlyContainAatfs()
        {
            var aatfList = SetupAatfList();
            var facilityType = FacilityType.Aatf;

            var result = aatfDataAatfDataFilter.Filter(aatfList, facilityType, true);

            result.Should().NotBeEmpty();
            result.Should().OnlyContain(m => m.FacilityType == FacilityType.Aatf);
        }

        [Fact]
        public void Filter_GivenAesAndAeFacilityType_AatfListShouldOnlyContainAes()
        {
            var aatfList = SetupAatfList();
            var facilityType = FacilityType.Ae;

            var result = aatfDataAatfDataFilter.Filter(aatfList, facilityType, true);

            result.Should().NotBeEmpty();
            result.Should().OnlyContain(m => m.FacilityType == FacilityType.Ae);
        }

        [Fact]
        public void Filter_GivenDisplayStatusTrue_ShouldAppendStatus()
        {
            var aatfList = new List<AatfData>();
            var aatfId = Guid.NewGuid();
            var facilityType = FacilityType.Aatf;

            var aatfData = new AatfData(Guid.NewGuid(), "AATF", "approval number", 2019, A.Dummy<Core.Shared.UKCompetentAuthorityData>(),
                Core.AatfReturn.AatfStatus.Approved, A.Dummy<AatfAddressData>(), Core.AatfReturn.AatfSize.Large, DateTime.Now,
                A.Dummy<Core.Shared.PanAreaData>(), null)
            {
                FacilityType = facilityType,
                AatfId = aatfId,
            };

            aatfList.Add(aatfData);

            var result = aatfDataAatfDataFilter.Filter(aatfList, facilityType, true);

            result[0].AatfContactDetailsName.Should().Contain(aatfData.AatfStatus.ToString());
        }

        [Fact]
        public void Filter_GivenDisplayStatusFalse_ShouldNotAppendStatus()
        {
            var aatfList = new List<AatfData>();
            var aatfId = Guid.NewGuid();
            var facilityType = FacilityType.Aatf;

            var aatfData = new AatfData(Guid.NewGuid(), "AATF", "approval number", 2019, A.Dummy<Core.Shared.UKCompetentAuthorityData>(),
                Core.AatfReturn.AatfStatus.Approved, A.Dummy<AatfAddressData>(), Core.AatfReturn.AatfSize.Large, DateTime.Now,
                A.Dummy<Core.Shared.PanAreaData>(), null)
            {
                FacilityType = facilityType,
                AatfId = aatfId,
            };

            aatfList.Add(aatfData);

            var result = aatfDataAatfDataFilter.Filter(aatfList, facilityType, false);

            result[0].AatfContactDetailsName.Should().NotContain(aatfData.AatfStatus.ToString());
        }

        private List<AatfData> SetupAatfList()
        {
            var aatfList = new List<AatfData>();

            var aatfData = new AatfData(Guid.NewGuid(), "AATF", "approval number", 2019, A.Dummy<Core.Shared.UKCompetentAuthorityData>(),
                Core.AatfReturn.AatfStatus.Approved, A.Dummy<AatfAddressData>(), Core.AatfReturn.AatfSize.Large, DateTime.Now,
                A.Dummy<Core.Shared.PanAreaData>(), null)
            {
                FacilityType = FacilityType.Aatf,
                AatfId = Guid.NewGuid()
            };

            var aatfData2 = new AatfData(Guid.NewGuid(), "AATF2", "approval number", 2019, A.Dummy<Core.Shared.UKCompetentAuthorityData>(),
                Core.AatfReturn.AatfStatus.Approved, A.Dummy<AatfAddressData>(), Core.AatfReturn.AatfSize.Large, DateTime.Now,
                A.Dummy<Core.Shared.PanAreaData>(), null)
            {
                FacilityType = FacilityType.Aatf,
                AatfId = Guid.NewGuid()
            };

            var exporterData = new AatfData(Guid.NewGuid(), "AE", "approval number", 2019, A.Dummy<Core.Shared.UKCompetentAuthorityData>(),
                Core.AatfReturn.AatfStatus.Approved, A.Dummy<AatfAddressData>(), Core.AatfReturn.AatfSize.Large, DateTime.Now,
                A.Dummy<Core.Shared.PanAreaData>(), null)
            {
                FacilityType = FacilityType.Ae,
                AatfId = Guid.NewGuid()
            };

            var exporterData2 = new AatfData(Guid.NewGuid(), "AE", "approval number", 2019, A.Dummy<Core.Shared.UKCompetentAuthorityData>(),
                Core.AatfReturn.AatfStatus.Approved, A.Dummy<AatfAddressData>(), Core.AatfReturn.AatfSize.Large, DateTime.Now,
                A.Dummy<Core.Shared.PanAreaData>(), null)
            {
                FacilityType = FacilityType.Ae,
                AatfId = Guid.NewGuid()
            };

            aatfList.Add(aatfData);
            aatfList.Add(exporterData);
            aatfList.Add(aatfData2);
            aatfList.Add(exporterData2);
            return aatfList;
        }
    }
}
