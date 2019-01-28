namespace EA.Weee.Web.Areas.AatfReturn.Requests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using ViewModels;
    using Web.Requests.Base;
    using Weee.Requests.AatfReturn;

    public class NonObligatedWeeRequestCreator : RequestCreator<NonObligatedValuesViewModel, NonObligatedRequest>, INonObligatedWeeRequestCreator
    {
        public override NonObligatedRequest ViewModelToRequest(NonObligatedValuesViewModel viewModel)
        {
            // Auto mappings
            var request = base.ViewModelToRequest(viewModel);

            return request;
        }
    }
}