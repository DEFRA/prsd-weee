namespace EA.Weee.Tests.Core
{
    using System;
    using Weee.Core.AatfReturn;
    using Weee.Core.DataReturns;

    public static class QuarterWindowTestHelper
    {
        public static QuarterWindow GetDefaultQuarterWindow()
        {
            return new QuarterWindow(new DateTime(2019, 04, 01), new DateTime(2020, 03, 16), QuarterType.Q1);
        }

        public static QuarterWindow GetQuarterFourWindow(int year)
        {
            return new QuarterWindow(new DateTime(year, 10, 01), new DateTime(year + 1, 03, 16), QuarterType.Q4);
        }
    }
}
