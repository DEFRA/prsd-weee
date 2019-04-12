namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using System;
    using System.Collections.Generic;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Shared;

    public class SiteAddressDataToReusedOffSiteCreateSiteViewModelMapTransfer
    {
        public Guid OrganisationId { get; set; }

        public Guid ReturnId { get; set; }

        public Guid AatfId { get; set; }

        public Guid? SiteId { get; set; }

        public AddressTonnageSummary ReturnedSites { get; set; }

        public IList<CountryData> Countries { get; set; }
    }
}