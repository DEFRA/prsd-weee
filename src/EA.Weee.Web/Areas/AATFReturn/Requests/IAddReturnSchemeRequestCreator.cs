namespace EA.Weee.Web.Areas.AatfReturn.Requests
{
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Requests.Base;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public interface IAddReturnSchemeRequestCreator : IRequestCreator<SelectYourPCSViewModel, List<AddReturnScheme>>
    {
    }
}