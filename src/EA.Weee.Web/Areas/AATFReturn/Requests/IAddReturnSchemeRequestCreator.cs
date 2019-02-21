namespace EA.Weee.Web.Areas.AatfReturn.Requests
{
    using System.Collections.Generic;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Requests.Base;

    public interface IAddReturnSchemeRequestCreator : IRequestCreator<SelectYourPCSViewModel, List<AddReturnScheme>>
    {
    }
}