namespace EA.Weee.Web.Areas.AatfEvidence.Mappings.ToViewModel
{
    using EA.Weee.Core.AatfReturn;
    using System;
    using System.Collections.Generic;

    public class AatfDataToSelectYourAatfViewModelMapTransfer
    {
        public Guid OrganisationId { get; set; }

        public List<AatfData> AatfList { get; set; }

        public FacilityType FacilityType { get; set; }
    }
}