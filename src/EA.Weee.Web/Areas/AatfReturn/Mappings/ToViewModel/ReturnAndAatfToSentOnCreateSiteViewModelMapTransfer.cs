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
        public Guid OrganisationId;

        public Guid ReturnId;

        public Guid AatfId;

        public Guid? WeeeSentOnId;

        public AatfAddressData SiteAddressData;

        public Guid? SiteAddressId;

        public AatfAddressData OperatorAddressData;

        public Guid? OperatorAddressId;

        public IList<Core.Shared.CountryData> CountryData;

        public bool? JavascriptDisabled;

        public ReturnAndAatfToSentOnCreateSiteViewModelMapTransfer()
        {
        }
    }
}