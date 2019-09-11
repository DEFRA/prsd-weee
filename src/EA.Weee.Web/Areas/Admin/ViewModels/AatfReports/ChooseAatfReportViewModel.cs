namespace EA.Weee.Web.Areas.Admin.ViewModels.AatfReports
{
    using System.Collections.Generic;
    using Reports;
    using Web.ViewModels.Shared;

    public class ChooseAatfReportViewModel : RadioButtonStringCollectionViewModel
    {
        public ChooseAatfReportViewModel()
            : base(new List<string>
            {
                Reports.AatfObligatedData,
                Reports.AatfReuseSitesData,
                Reports.AatfSentOnData,
                Reports.AatfNonObligatedData,
                Reports.UkWeeeDataAtAatfs,
                Reports.UkNonObligatedWeeeData,
                Reports.AatfAePublicRegister,
                Reports.AatfAeReturnData
            })
        {
        }
    }
}