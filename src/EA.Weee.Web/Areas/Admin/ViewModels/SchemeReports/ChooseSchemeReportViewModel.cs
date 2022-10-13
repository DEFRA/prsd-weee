namespace EA.Weee.Web.Areas.Admin.ViewModels.SchemeReports
{
    using System.Collections.Generic;
    using Reports;
    using Web.ViewModels.Shared;

    public class ChooseSchemeReportViewModel : RadioButtonStringCollectionViewModel
    {
        public ChooseSchemeReportViewModel()
            : base(new List<string>
            {
                Reports.ProducerDetails,
                Reports.ProducerEeeData,
                Reports.SchemeWeeeData,
                Reports.UkEeeData,
                Reports.UkWeeeData,
                Reports.ProducerPublicRegister,
                Reports.SchemeObligationData,
                Reports.MissingProducerData,
                Reports.PcsEvidenceAndObligationProgressData
            })
        {
        }
    }
}