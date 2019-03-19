namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using EA.Weee.Core.AatfReturn;
    using System;

    public class SentOnCreateSiteViewModel
    {
        public Guid OrganisationId { get; set; }

        public Guid ReturnId { get; set; }

        public Guid AatfId { get; set; }

        public AatfAddressData SiteAddressData { get; set; }

        public AatfAddressData OperatorAddressData { get; set; }
    }
}