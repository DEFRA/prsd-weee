namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using System;
    using System.Collections.Generic;
    using Core.AatfReturn;

    public class AatfEvidenceToSelectYourAatfViewModelMapTransfer
    {
        public Guid OrganisationId { get; set; }

        public List<AatfData> AatfList { get; set; }

        public DateTime CurrentDate { get; set; }

        public DateTime EvidenceSiteSelectionStartDateFrom { get; set; }
    }
}