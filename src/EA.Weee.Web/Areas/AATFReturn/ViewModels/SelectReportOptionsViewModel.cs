namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.DataReturns;

    public class SelectReportOptionsViewModel : ReturnViewModelBase
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
            int year) : base(quarter, window, year)
        {
            OrganisationId = organisationId;
            ReturnId = returnId;
            ReportOnQuestions = reportOnQuestions;
            Year = year.ToString();
        }

        public Guid OrganisationId { get; set; }

        public Guid ReturnId { get; set; }

        public List<ReportOnQuestion> ReportOnQuestions { get; set; }

        public List<int> SelectedOptions { get; set; }

        public string DcfSelectedValue { get; set; }

        public IList<string> DcfPossibleValues => new List<string> { "Yes", "No" };

        public override string Period => $"{Quarter} {QuarterWindow.StartDate.ToString("MMM", CultureInfo.CurrentCulture)} - {QuarterWindow.EndDate.ToString("MMM", CultureInfo.CurrentCulture)}";

        public override string Year { get; }
    }
}