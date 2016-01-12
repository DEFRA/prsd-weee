namespace EA.Weee.Domain.DataReturns
{
    using System;

    public class QuarterWindow
    {
        public DateTime StartDate { get; private set; }

        public DateTime EndDate { get; private set; }

        public QuarterWindow(DateTime startDate, DateTime endDate)
        {
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
