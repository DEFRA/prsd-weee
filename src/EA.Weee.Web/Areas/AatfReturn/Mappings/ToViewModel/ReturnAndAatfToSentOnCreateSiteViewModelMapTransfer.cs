namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Requests.Shared;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

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