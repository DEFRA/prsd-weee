namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using EA.Weee.Core.AatfReturn;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class ReturnAndAatfToSentOnCreateSiteOperatorViewModelMapTransfer
    {
        public Guid OrganisationId;

        public Guid ReturnId;

        public Guid AatfId;

        public Guid WeeeSentOnId;

        public Guid? OperatorAddressId;

        public bool? JavascriptDisabled;

        public AatfAddressData OperatorAddressData;

        public AatfAddressData SiteAddressData;

        public IList<Core.Shared.CountryData> CountryData;

        public ReturnAndAatfToSentOnCreateSiteOperatorViewModelMapTransfer()
        {
        }
    }
}