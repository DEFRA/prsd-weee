namespace EA.Weee.Web.Tests.Unit.Infrastructure
{
    using System;
    using Core.DataReturns;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Infrastructure;
    using Weee.Tests.Core;
    using Xunit;

    public class DisplayHelperTests
    {
        [Fact]
        public void FormatQuarter_BasedOnParameters()
        {
            var quarterData = new Quarter(2019, QuarterType.Q1);
            var quarterWindow = QuarterWindowTestHelper.GetDefaultQuarterWindow();

            var value = DisplayHelper.FormatQuarter(quarterData, quarterWindow);

            Assert.Equal("2019 Q1 Jan - Mar", value);
        }

        [Fact]
        public void FormatQuarter_ReturnsEmptyString()
        {
            var value = DisplayHelper.FormatQuarter(null, null);

            Assert.Equal(string.Empty, value);
        }

        [Fact]
        public void ReturnDisplays_EmptyString()
        {
            var value = DisplayHelper.ReportingOnValue(string.Empty, string.Empty);

            Assert.Equal(string.Empty, value);
        }

        [Fact]
        public void ReturnsDisplay_AatfInfo()
        {
            var name = "AATF Test 1";
            var approvalNumber = "WEE/TT1234PP/ATF";

            var value = DisplayHelper.ReportingOnValue(name, approvalNumber);

            Assert.Equal("AATF Test 1 (WEE/TT1234PP/ATF)", value);
        }
    }
}
