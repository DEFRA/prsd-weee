namespace EA.Weee.Core.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class QuarterWindow
    {
        public DateTime StartDate { get; private set; }

        public DateTime EndDate { get; private set; }

        public QuarterWindow(DateTime startDate, DateTime endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }
    }
}
