namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using EA.Weee.Core.AatfReturn;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class SentOnCreateSiteOperatorViewModel : SentOnCreateSiteViewModel
    {
        public AddressData OperatorAddressData;

        public Guid WeeeSentOnId;
    }
}