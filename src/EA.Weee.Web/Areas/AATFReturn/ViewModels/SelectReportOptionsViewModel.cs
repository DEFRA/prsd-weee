namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using EA.Prsd.Core;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.DataReturns;

    public class SelectReportOptionsViewModel
    {
        public SelectReportOptionsViewModel()
        {
        }

        public SelectReportOptionsViewModel(
            Guid organisationId,
            Guid returnId,
            List<ReportOnQuestion> reportOnQuestions,
            Quarter quarter,
            QuarterWindow window,
            int year)
        {
            Guard.ArgumentNotNull(() => quarter, quarter);
            Guard.ArgumentNotNull(() => window, window);

            OrganisationId = organisationId;
            ReturnId = returnId;
            ReportOnQuestions = reportOnQuestions;
            this.QuarterWindow = window;
            this.quarter = quarter;
            Year = year.ToString();
        }

        protected readonly QuarterWindow QuarterWindow;
        private readonly Quarter quarter;
        private readonly int year;

        public Guid OrganisationId { get; set; }

        public Guid ReturnId { get; set; }

        public List<ReportOnQuestion> ReportOnQuestions { get; set; }

        public List<int> SelectedOptions { get; set; }

        public string DcfSelectedValue { get; set; }

        public IList<string> DcfPossibleValues => new List<string> { "Yes", "No" };

        public string Quarter => quarter.Q.ToString();

        public string Period => $"{Quarter} {QuarterWindow.StartDate.ToString("MMM", CultureInfo.CurrentCulture)} - {QuarterWindow.EndDate.ToString("MMM", CultureInfo.CurrentCulture)}";

        public string Year { get; }
    }
}