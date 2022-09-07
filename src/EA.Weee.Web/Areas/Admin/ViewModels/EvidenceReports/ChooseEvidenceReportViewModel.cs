namespace EA.Weee.Web.Areas.Admin.ViewModels.EvidenceReports
{
    using EA.Weee.Web.ViewModels.Shared;
    using Reports;
    using System.Collections.Generic;

    public class ChooseEvidenceReportViewModel : RadioButtonStringCollectionViewModel
    {
        public ChooseEvidenceReportViewModel()
            : base(new List<string>
            {
                Reports.EvidenceNoteData,
                Reports.EvidenceNotesReports
            })
        {
        }
    }
}