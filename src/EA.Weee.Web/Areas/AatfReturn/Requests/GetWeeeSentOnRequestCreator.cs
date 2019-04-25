namespace EA.Weee.Web.Areas.AatfReturn.Requests
{
    using EA.Prsd.Core;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Requests.Base;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class GetWeeeSentOnRequestCreator : RequestCreator<SentOnSiteSummaryListViewModel, GetWeeeSentOn>, IGetWeeeSentOnRequestCreator
    {
        public override GetWeeeSentOn ViewModelToRequest(SentOnSiteSummaryListViewModel viewModel)
        {
            Guard.ArgumentNotNull(() => viewModel, viewModel);

            return new GetWeeeSentOn(viewModel.AatfId, viewModel.ReturnId, null);
        }
    }
}