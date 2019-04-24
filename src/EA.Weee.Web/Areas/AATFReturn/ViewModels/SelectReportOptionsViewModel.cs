namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using System;
    using System.Collections.Generic;
    using EA.Prsd.Core;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.ViewModels.Shared;

    public class SelectReportOptionsViewModel : RadioButtonStringCollectionViewModel
    {
        public SelectReportOptionsViewModel() : base(new List<string> { "Yes", "No" })
        {
        }

        public Guid OrganisationId { get; set; }

        public Guid ReturnId { get; set; }

        public List<ReportOnQuestion> ReportOnQuestions { get; set; }
    }
}