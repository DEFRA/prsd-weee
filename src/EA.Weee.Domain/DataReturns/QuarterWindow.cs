namespace EA.Weee.Domain.DataReturns
{
    using System;

    public class QuarterWindow
    {
        public DateTime StartDate { get; private set; }

        public DateTime EndDate { get; private set; }

        public QuarterType QuarterType { get; private set; } 

        public QuarterWindow(DateTime startDate, DateTime endDate, QuarterType quarterType)
        {
            StartDate = startDate;
            EndDate = endDate;
            QuarterType = quarterType;
        }

        public bool IsBeforeWindow(DateTime date)
        {
            return date.Date.ToUniversalTime() < StartDate.Date.ToUniversalTime();
        }

        public bool IsAfterWindow(DateTime date)
        {
            return date.Date.ToUniversalTime() > EndDate.Date.ToUniversalTime();
        }

        public bool IsInWindow(DateTime date)
        {
            return (!IsBeforeWindow(date) && !IsAfterWindow(date));
        }
    }
}
