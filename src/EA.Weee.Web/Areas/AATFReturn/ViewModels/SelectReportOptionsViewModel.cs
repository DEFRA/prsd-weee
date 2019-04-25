namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using System;
    using System.Collections.Generic;
    using EA.Prsd.Core;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.ViewModels.Shared;

    public class SelectReportOptionsViewModel
    {
        public Guid OrganisationId { get; set; }

        public Guid ReturnId { get; set; }

        public List<ReportOnQuestion> ReportOnQuestions { get; set; }

        public List<int> SelectedOptions { get; set; }

        public string DcfSelectedValue { get; set; }

        public IList<string> DcfPossibleValues => new List<string> { "Yes", "No" };
    }
}