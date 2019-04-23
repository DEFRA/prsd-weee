namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using System;
    using System.Collections.Generic;
    using EA.Prsd.Core;
    using EA.Weee.Core.AatfReturn;

    public class SelectReportOptionsViewModel
    {
        public SelectReportOptionsViewModel()
        {
        }

        public Guid OrganisationId { get; set; }

        public Guid ReturnId { get; set; }

        public List<ReportOnQuestion> ReportOnQuestions { get; set; }
    }
}