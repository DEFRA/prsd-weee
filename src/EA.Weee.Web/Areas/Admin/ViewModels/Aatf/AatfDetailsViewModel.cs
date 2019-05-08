namespace EA.Weee.Web.Areas.Admin.ViewModels.Aatf
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Shared;
    using System;
    public class AatfDetailsViewModel
    {
        public Guid AatfId { get; set; }

        public string AatfName { get; set; }
        public string ApprovalNumber { get; set; }

        public UKCompetentAuthorityData CompetentAuthority { get; set; }

        public AatfStatus AatfStatus { get; set; }
        public virtual AatfAddressData SiteAddress { get; set; }
        public AatfSize Size { get; set; }
        public DateTime? ApprovalDate { get; set; }
    }
}