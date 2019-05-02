namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using EA.Weee.Core.AatfReturn;
    using System;
    using System.ComponentModel;

    public class SentOnCreateSiteOperatorViewModel : SentOnCreateSiteViewModel
    {
        public OperatorAddressData OperatorAddressData { get; set; }

        [DisplayName("Is the operator address the same as the AATF/ATF address?")]
        public bool IsOperatorTheSameAsAATF { get; set; }

        public bool OperatorAddressFound { get; set; }

        public Guid OperatorAddressId { get; set; }
    }
}