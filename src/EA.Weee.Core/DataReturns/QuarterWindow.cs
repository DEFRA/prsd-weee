namespace EA.Weee.Core.DataReturns
{
    using System;

    public class QuarterWindow
    {
        public Quarter Quarter { get; private set; }

        public DateTime StartDate { get; private set; }

        public DateTime EndDate { get; private set; }

        public QuarterWindow(Quarter quarter, DateTime startDate, DateTime endDate)
        {
            Quarter = quarter;
            StartDate = startDate;
            EndDate = endDate;
        }

        public bool IsBeforeWindow(DateTime date)
        {
            return date.Date.ToUniversalTime() < StartDate.Date.ToUniversalTime();
        }

        public bool IsAfterWindow(DateTime date)
        {
            return date.Date.ToUniversalTime() > EndDate.Date.ToUniversalTime();
        }
    }
}
