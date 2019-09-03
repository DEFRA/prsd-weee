namespace EA.Weee.Web.Areas.Admin.Mappings.ToViewModel
{
    using System;
    using System.Collections.Generic;
    using Core.Admin.AatfReports;
    using EA.Weee.Core.AatfReturn;

    public class AatfDataToAatfDetailsViewModelMapTransfer
    {
        public AatfDataToAatfDetailsViewModelMapTransfer(AatfData aatfData)
        {
            AatfData = aatfData;
        }

        public AatfData AatfData { get; set; }

        public string OrganisationString { get; set; }

        public List<AatfDataList> AssociatedAatfs { get; set; }

        public List<Core.Scheme.SchemeData> AssociatedSchemes { get; set; }

        public List<AatfSubmissionHistoryData> SubmissionHistory { get; set; }

        public IEnumerable<short> ComplianceYearList { get; set; }

        public DateTime CurrentDate { get; set; }
    }
}