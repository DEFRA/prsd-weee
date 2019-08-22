namespace EA.Weee.Core.AatfReturn
{
    using DataReturns;
    using System;

    public class QuarterWindow
    {
        public DateTime WindowOpenDate { get; private set; }

        public DateTime QuarterEnd { get; private set; }

        public DateTime QuarterStart { get; private set; }

        public DateTime WindowClosedDate { get; private set; }

        public QuarterType QuarterType { get; private set; }

        public QuarterWindow(DateTime windowOpenDate, DateTime windowClosedDate, QuarterType quarterType)
        {
            int startMonth;
            int startYear;
            if (quarterType == QuarterType.Q4)
            {
                startMonth = 10;
                startYear = windowOpenDate.Year - 1;
            }
            else
            {
                startMonth = windowOpenDate.Month - 3;
                startYear = windowOpenDate.Year;
            }

            QuarterStart = new DateTime(startYear, startMonth, 1);
            QuarterEnd = windowOpenDate.AddDays(-1);

            WindowOpenDate = windowOpenDate;
            WindowClosedDate = windowClosedDate;

            QuarterType = quarterType;
        }

        public bool IsOpen(DateTime date)
        {
            return date.ToUniversalTime() >= WindowOpenDate.ToUniversalTime() && date.ToUniversalTime() <= WindowClosedDate.ToUniversalTime();
        }
    }
}
