namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using EA.Weee.Core.AatfReturn;
    using System;
    using System.Collections.Generic;

    public class ReturnAndAatfToSentOnCreateSiteViewModelMapTransfer
    {
        public ReturnData Return { get; set; }

        public Guid AatfId { get; set; }

        public WeeeSentOnData WeeeSentOnData { get; set; }

        public IList<Core.Shared.CountryData> CountryData { get; set; }

        public AatfAddressData SiteAddressData { get; set; }

        public ReturnAndAatfToSentOnCreateSiteViewModelMapTransfer()
        {
        }
    }
}