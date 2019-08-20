namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using EA.Weee.Core.AatfReturn;
    using System;
    using System.Collections.Generic;

    public class AatfDataToHomeViewModelMapTransfer
    {
        public Guid OrganisationId { get; set; }

        public List<AatfData> AatfList { get; set; }

        public bool IsAE { get; set; }
    }
}