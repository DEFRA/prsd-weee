namespace EA.Weee.Web.Areas.Admin.Mappings.ToViewModel
{
    using EA.Weee.Core.AatfReturn;
    using System.Collections.Generic;
    using Core.Admin;
    using Core.Admin.AatfReports;

    public class AatfDataToAatfDetailsViewModelMapTransfer
    {
        public AatfDataToAatfDetailsViewModelMapTransfer(AatfData aatfData)
        {
            AatfData = aatfData;
        }

        public AatfData AatfData { get; set; }

        public string OrganisationString { get; set; }

        public string SiteAddressString { get; set; }

        public string ContactAddressString { get; set; }

        public List<AatfDataList> AssociatedAatfs { get; set; }

        public List<Core.Scheme.SchemeData> AssociatedSchemes { get; set; }

        public List<AatfSubmissionHistoryData> SubmissionHistory { get; set; }
    }
}