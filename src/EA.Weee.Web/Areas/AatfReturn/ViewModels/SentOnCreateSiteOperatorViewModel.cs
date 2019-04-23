namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using EA.Weee.Core.AatfReturn;
    using System;
    using System.ComponentModel;

    public class SentOnCreateSiteOperatorViewModel : SentOnCreateSiteViewModel
    {
        public OperatorAddressData OperatorAddressData { get; set; }

        [DisplayName("The operator address is the same as the AATF/ATF address")]
        public bool IsOperatorTheSameAsAATF { get; set; }
    }
}