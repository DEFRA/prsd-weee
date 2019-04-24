namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Mapping.ToViewModel
{
    using System;
    using System.Collections.Generic;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class AddressTonnageSummaryToReusedOffSiteSummaryListViewModelMapTests
    {
        private readonly AddressTonnageSummaryToReusedOffSiteSummaryListViewModelMap map;
        private readonly AatfData testAatf;
        private readonly List<SiteAddressData> testAddressDataList;
        private readonly List<WeeeObligatedData> testObligatedDataList;
        private readonly string nullTonnageDisplay = "-";

        public AddressTonnageSummaryToReusedOffSiteSummaryListViewModelMapTests()
        {
            map = new AddressTonnageSummaryToReusedOffSiteSummaryListViewModelMap(new TonnageUtilities(), new AddressUtilities());
            testAatf = new AatfData(Guid.NewGuid(), "Test Aatf", "Aatf approval");
            testAddressDataList = new List<SiteAddressData>();
            testObligatedDataList = new List<WeeeObligatedData>();
        }

        [Fact]
        public void Map_GivenNullSource_ArgumentNullExceptionExpected()
        {
            Action action = () => map.Map(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenValidSource_PropertiesShouldBeMapped()
        {
            testAddressDataList.Add(new SiteAddressData(
                "Name",
                "Address1",
                "Address2",
                "Town",
                "County",
                "PO12 3ST",
                A.Dummy<Guid>(),
                "Country"));

            testObligatedDataList.Add(new WeeeObligatedData(Guid.NewGuid(), null, testAatf, 0, 1.234m, 1.234m));

            var returnData = new AddressTonnageSummary()
            {
                AddressData = testAddressDataList,
                ObligatedData = testObligatedDataList
            };

            var result = map.Map(returnData);

            result.Addresses[0].Name.Should().Be("Name");
            result.Addresses[0].Address.Should().Be("Address1, Address2, Town, County, PO12 3ST, Country");
            result.B2bTotal.Should().Be("1.234");
            result.B2bTotal.Should().Be("1.234");
        }

        [Fact]
        public void Map_GivenNullObligatedData_ReturnsNullTonnageDisplay()
        {
            testAddressDataList.Add(A.Fake<SiteAddressData>());

            testObligatedDataList.Add(new WeeeObligatedData(Guid.NewGuid(), null, testAatf, 0, null, null));

            var returnData = new AddressTonnageSummary()
            {
                AddressData = testAddressDataList,
                ObligatedData = testObligatedDataList
            };

            var result = map.Map(returnData);

            result.B2bTotal.Should().Be(nullTonnageDisplay);
            result.B2bTotal.Should().Be(nullTonnageDisplay);
        }

        [Fact]
        public void Map_GivenZeroValuelObligatedData_ReturnsZero()
        {
            testAddressDataList.Add(A.Fake<SiteAddressData>());

            testObligatedDataList.Add(new WeeeObligatedData(Guid.NewGuid(), null, testAatf, 0, 0, 0));

            var returnData = new AddressTonnageSummary()
            {
                AddressData = testAddressDataList,
                ObligatedData = testObligatedDataList
            };

            var result = map.Map(returnData);

            result.B2bTotal.Should().Be("0.000");
            result.B2bTotal.Should().Be("0.000");
        }
    }
}
