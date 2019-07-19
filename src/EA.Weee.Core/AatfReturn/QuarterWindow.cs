namespace EA.Weee.Core.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using DataReturns;

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
            if (quarterType == QuarterType.Q4)
            {
                startMonth = 10;
            }
            else
            {
                startMonth = windowOpenDate.Month - 3;
            }

            QuarterStart = new DateTime(windowOpenDate.Year, startMonth, 1);
            QuarterEnd = windowOpenDate.AddDays(-1);
           
            WindowOpenDate = windowOpenDate;
            WindowClosedDate = windowClosedDate;

            QuarterType = quarterType;
        }

        public bool IsOpen(DateTime date)
        {
            return date >= WindowOpenDate && date <= WindowClosedDate;
        }
    }
}
