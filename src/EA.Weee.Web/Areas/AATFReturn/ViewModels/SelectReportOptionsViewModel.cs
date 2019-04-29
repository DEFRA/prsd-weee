namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using System;
    using System.Collections.Generic;
    using EA.Weee.Core.AatfReturn;

    public class SelectReportOptionsViewModel
    {
        public SelectReportOptionsViewModel()
        {
        }

        public SelectReportOptionsViewModel(Guid organisationId, Guid returnId, List<ReportOnQuestion> reportOnQuestions)
        {
            OrganisationId = organisationId;
            ReturnId = returnId;
            ReportOnQuestions = reportOnQuestions;
        }

        public Guid OrganisationId { get; set; }

        public Guid ReturnId { get; set; }

        public List<ReportOnQuestion> ReportOnQuestions { get; set; }

        public List<int> SelectedOptions { get; set; }

        public string DcfSelectedValue { get; set; }

        public IList<string> DcfPossibleValues => new List<string> { "Yes", "No" };
    }
}