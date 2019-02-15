namespace EA.Weee.Web.Areas.AatfReturn.Requests
{
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Requests.Base;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class AddReturnSchemeRequestCreator : RequestCreator<SelectYourPCSViewModel, List<AddReturnScheme>>, IAddReturnSchemeRequestCreator
    {
        public override List<AddReturnScheme> ViewModelToRequest(SelectYourPCSViewModel viewModel)
        {
            List<AddReturnScheme> requestList = new List<AddReturnScheme>();

            foreach (var scheme in viewModel.SelectedSchemes)
            {
                var returnSchemeRequest = new AddReturnScheme()
                {
                    SchemeId = scheme,
                    ReturnId = viewModel.ReturnId
                };

                requestList.Add(returnSchemeRequest);
            }

            return requestList;
        }
    }
}