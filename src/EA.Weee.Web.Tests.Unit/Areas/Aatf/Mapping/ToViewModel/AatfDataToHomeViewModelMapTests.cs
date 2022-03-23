namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Mapping.ToViewModel
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using Web.Areas.Aatf.Mappings.Filters;
    using Xunit;

    public class AatfDataToHomeViewModelMapTests
    {
        private readonly AatfDataToHomeViewModelMap map;
        private readonly Fixture fixture;
        private readonly IAatfDataFilter<List<AatfData>, FacilityType> filter;

        public AatfDataToHomeViewModelMapTests()
        {
            fixture = new Fixture();
            filter = A.Fake<IAatfDataFilter<List<AatfData>, FacilityType>>();

            map = new AatfDataToHomeViewModelMap(filter);
        }

        [Fact]
        public void Map_GivenNullSource_ArgumentNullExceptionExpected()
        {
            Action action = () => map.Map(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenListOfAatfs_MapperShouldBeCalled()
        {
            //arrange
            var aatfList = fixture.CreateMany<AatfData>().ToList();
            var facility = fixture.Create<FacilityType>();
            var transfer = new AatfDataToHomeViewModelMapTransfer() { OrganisationId = Guid.NewGuid(), AatfList = aatfList, FacilityType = facility };

            //act
            map.Map(transfer);

            //assert
            A.CallTo(() => filter.Filter(aatfList, facility)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Map_GivenListOfAatfs_ListIdOrderedByName()
        {
            var organisationId = Guid.NewGuid();
            var aatfList = new List<AatfData>();

            var aatfData = new AatfData(Guid.NewGuid(), "Banana", "approval number", 2019, A.Dummy<Core.Shared.UKCompetentAuthorityData>(),
                   Core.AatfReturn.AatfStatus.Approved, A.Dummy<AatfAddressData>(), Core.AatfReturn.AatfSize.Large, DateTime.Now,
                   A.Dummy<Core.Shared.PanAreaData>(), null)
            {
                FacilityType = FacilityType.Aatf
            };

            var aatfData2 = new AatfData(Guid.NewGuid(), "Carrot", "approval number", 2019, A.Dummy<Core.Shared.UKCompetentAuthorityData>(),
               Core.AatfReturn.AatfStatus.Approved, A.Dummy<AatfAddressData>(), Core.AatfReturn.AatfSize.Large, DateTime.Now,
               A.Dummy<Core.Shared.PanAreaData>(), null)
            {
                FacilityType = FacilityType.Aatf
            };

            var aatfData3 = new AatfData(Guid.NewGuid(), "Apple", "approval number", 2019, A.Dummy<Core.Shared.UKCompetentAuthorityData>(),
               Core.AatfReturn.AatfStatus.Approved, A.Dummy<AatfAddressData>(), Core.AatfReturn.AatfSize.Large, DateTime.Now,
               A.Dummy<Core.Shared.PanAreaData>(), null)
            {
                FacilityType = FacilityType.Aatf
            };

            aatfList.Add(aatfData);
            aatfList.Add(aatfData2);
            aatfList.Add(aatfData3);

            A.CallTo(() => filter.Filter(aatfList, A<FacilityType>.Ignored)).Returns(aatfList);

            var transfer = new AatfDataToHomeViewModelMapTransfer() { OrganisationId = organisationId, AatfList = aatfList, FacilityType = fixture.Create<FacilityType>() };

            var result = map.Map(transfer);

            result.AatfList.Should().BeInAscendingOrder(z => z.Name);
        }

        [Fact]
        public void Map_GivenSource_PropertiesShouldBeSet()
        {
            var transfer = new AatfDataToHomeViewModelMapTransfer() { OrganisationId = Guid.NewGuid(), AatfList = A.Fake<List<AatfData>>(), FacilityType = fixture.Create<FacilityType>() };

            var result = map.Map(transfer);

            result.AatfList.Should().BeEquivalentTo(transfer.AatfList);
            result.OrganisationId.Should().Be(transfer.OrganisationId);
            result.FacilityType.Should().Be(transfer.FacilityType);
        }
    }
}
