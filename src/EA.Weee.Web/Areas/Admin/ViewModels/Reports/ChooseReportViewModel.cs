namespace EA.Weee.Web.Areas.Admin.ViewModels.Reports
{
    using System.Collections.Generic;
    using Web.ViewModels.Shared;

    public class ChooseReportViewModel : RadioButtonStringCollectionViewModel
    {
        public ChooseReportViewModel()
            : base(new List<string>
            {
                Reports.ProducerDetails,
            Reports.ProducerPublicRegister,
            Reports.UKEEEData,
                Reports.UKWeeeData,
            Reports.ProducerEeeData,
                Reports.SchemeWeeeData
            })
        {
        }
    }
}