namespace EA.Weee.Web.Areas.AatfReturn.Requests
{
    using EA.Weee.Requests.AatfReturn.Obligated;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using System;

    public class GetSentOnAatfSiteRequestCreator : IGetSentOnAatfSiteRequestCreator
    {
        public GetSentOnAatfSite ViewModelToRequest(SentOnCreateSiteOperatorViewModel viewModel)
        {
            var aatfSite = new GetSentOnAatfSite((Guid)viewModel.WeeeSentOnId);

            return aatfSite;
        }
    }
}