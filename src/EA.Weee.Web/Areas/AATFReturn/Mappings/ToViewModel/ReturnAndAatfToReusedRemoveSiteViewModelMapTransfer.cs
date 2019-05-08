namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using EA.Weee.Core.AatfReturn;
    using System;

    public class ReturnAndAatfToReusedRemoveSiteViewModelMapTransfer
    {
        public Guid AatfId { get; set; }

        public Guid SiteId { get; set; }

        public Guid ReturnId { get; set; }

        public Guid OrganisationId { get; set; }

        public string SiteAddress { get; set; }

        public SiteAddressData Site { get; set; }
    }
}