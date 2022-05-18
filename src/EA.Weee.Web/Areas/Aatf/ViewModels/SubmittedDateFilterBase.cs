namespace EA.Weee.Web.Areas.Aatf.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SubmittedDateFilterBase
    {
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public SubmittedDateFilterBase(DateTime? startDate, DateTime? endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }
    }
}
