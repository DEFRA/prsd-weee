namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Helper
{
    using System;
    using EA.Prsd.Core;
    using Web.Extensions;
    using Xunit;

    public class AatfHelperTests
    {
        [Theory]
        [InlineData("01/01/2020", new int[] { 2021, 2020, 2019 })]
        [InlineData("01/12/2020", new int[] { 2021, 2020, 2019 })]
        [InlineData("01/31/2020", new int[] { 2021, 2020, 2019 })]
        [InlineData("02/01/2020", new int[] { 2021, 2020 })]
        [InlineData("12/02/2020", new int[] { 2021, 2020 })]
        [InlineData("01/02/2021", new int[] { 2022, 2021, 2020 })]
        [InlineData("03/02/2021", new int[] { 2022, 2021 })]
        public void GetValidComplianceYear_BasedOnCurrentDate(DateTime currentDate, int[] yearList)
        {
            SystemTime.Freeze(new DateTime(currentDate.Year, currentDate.Month, currentDate.Day));
            var value = ComplianceYearHelper.FetchCurrentComplianceYears(currentDate);
            SystemTime.Unfreeze();
            Assert.Equal(yearList, value.ToArray());
        }

        [Theory]
        [InlineData("01/01/2020", new int[] { 2021, 2020, 2019 })]
        [InlineData("01/12/2020", new int[] { 2021, 2020, 2019 })]
        [InlineData("01/31/2020", new int[] { 2021, 2020, 2019 })]
        [InlineData("02/01/2020", new int[] { 2021, 2020, 2019 })]
        [InlineData("12/02/2020", new int[] { 2021, 2020, 2019 })]
        [InlineData("01/02/2021", new int[] { 2022, 2021, 2020 })]
        [InlineData("03/02/2021", new int[] { 2022, 2021, 2020 })]
        public void GetCurrentComplianceYear_BasedOnCurrentDate(DateTime currentDate, int[] yearList)
        {
            SystemTime.Freeze(new DateTime(currentDate.Year, currentDate.Month, currentDate.Day));
            var value = ComplianceYearHelper.FetchCurrentComplianceYears(currentDate, true);
            SystemTime.Unfreeze();
            Assert.Equal(yearList, value.ToArray());
        }
    }
}
