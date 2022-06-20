namespace EA.Weee.Web.Areas.Aatf.ViewModels
{
    using System;

    [Serializable]
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
