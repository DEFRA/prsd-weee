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

        public OperatorAddressData OperatorAddressData;

        public AatfAddressData SiteAddressData;

        public ReturnAndAatfToSentOnCreateSiteOperatorViewModelMapTransfer(IList<Core.Shared.CountryData> countryData)
        {
            this.OperatorAddressData = new OperatorAddressData();
            OperatorAddressData.Countries = countryData;
        }
    }
}