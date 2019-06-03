namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using System;
    using System.Globalization;

    public class SelectReportOptionsNilViewModel
    {
        public Guid ReturnId { get; set; }

        public Guid OrganisationId { get; set; }

        public DateTime QuarterWindowStartDate { get; set; }

        public DateTime QuarterWindowEndDate { get; set; }

        public string Quarter { get; set; }

        public string Period => $"{Quarter} {QuarterWindowStartDate.ToString("MMM", CultureInfo.CurrentCulture)} - {QuarterWindowEndDate.ToString("MMM", CultureInfo.CurrentCulture)}";

        public string Year { get; set; }
    }
}