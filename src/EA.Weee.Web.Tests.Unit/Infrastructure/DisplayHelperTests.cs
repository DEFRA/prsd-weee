namespace EA.Weee.Web.Tests.Unit.Infrastructure
{
    using System;
    using Core.DataReturns;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Infrastructure;
    using Xunit;

    public class DisplayHelperTests
    {
        [Fact]
        public void FormatQuarter_BasedOnParamaters()
        {
            var quarterData = new Quarter(2019, QuarterType.Q1);
            var quarterWindow = new QuarterWindow(new DateTime(2019, 1, 1), new DateTime(2019, 3, 30));

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
            var value = DisplayHelper.ReportingOnValue(string.Empty, string.Empty, string.Empty);

            Assert.Equal(string.Empty, value);
        }

        [Fact]
        public void ReturnDisplaysOnly_Quarter()
        {
            var quarter = "2019 Q1 Jan - Mar";

            var value = DisplayHelper.ReportingOnValue(string.Empty, string.Empty, quarter);

            Assert.Equal("<b>Reporting period: </b>2019 Q1 Jan - Mar&#09;", value);
        }

        [Fact]
        public void ReturnsDisplayBoth_Quarter_AatfInfo()
        {
            var quarter = "2019 Q1 Jan - Mar";
            var name = "AATF Test 1";
            var approvalNumber = "WEE/TT1234PP/ATF";

            var value = DisplayHelper.ReportingOnValue(name, approvalNumber, quarter);

            Assert.Equal("<b>Reporting period: </b>2019 Q1 Jan - Mar&#09;&#09;&#09;&#09; <b>Reporting on: </b>AATF Test 1 (WEE/TT1234PP/ATF)", value);
        }
    }
}
