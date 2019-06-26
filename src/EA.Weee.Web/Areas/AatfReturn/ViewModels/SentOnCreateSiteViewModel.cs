namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using EA.Weee.Core.AatfReturn;
    using System;
    using System.ComponentModel;

    public class SentOnCreateSiteViewModel
    {
        public Guid OrganisationId { get; set; }

        public Guid ReturnId { get; set; }

        public Guid AatfId { get; set; }

        public Guid? WeeeSentOnId { get; set; }

        public bool Edit => (WeeeSentOnId.HasValue);

        public AatfAddressData SiteAddressData { get; set; }

        public OperatorAddressData OperatorAddressData { get; set; }

        public Guid? SiteAddressId { get; set; }

        public Guid? OperatorAddressId { get; set; }

        [DisplayName("Is the operator address the same as the AATF/ATF address?")]
        public bool IsOperatorTheSameAsAATF { get; set; }
    }
}