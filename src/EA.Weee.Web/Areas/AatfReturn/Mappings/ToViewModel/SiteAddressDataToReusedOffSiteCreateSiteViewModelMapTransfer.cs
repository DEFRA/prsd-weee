namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Shared;
    using System;
    using System.Collections.Generic;

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