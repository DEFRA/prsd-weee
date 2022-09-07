namespace EA.Weee.Web.Areas.Admin.ViewModels.Reports
{
    using System.Collections.Generic;
    using Web.ViewModels.Shared;

    public class ChooseReportTypeModel : RadioButtonStringCollectionViewModel
    {
        public ChooseReportTypeModel()
            : base(new List<string>
            {
                Reports.PcsReports,
                Reports.AatfReports,
                Reports.PcsAatfDataDifference,
                Reports.AatfAeDetails,
                Reports.EvidenceNotesReports
            })
        {
        }
    }
}