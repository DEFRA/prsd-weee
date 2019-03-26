namespace EA.Weee.Web.Areas.AatfReturn.Requests
{
    using EA.Weee.Requests.AatfReturn.Obligated;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;

    public class GetSentOnAatfSiteRequestCreator : IGetSentOnAatfSiteRequestCreator
    {
        public GetSentOnAatfSite ViewModelToRequest(SentOnCreateSiteOperatorViewModel viewModel)
        {
            var aatfSite = new GetSentOnAatfSite(viewModel.WeeeSentOnId);

            return aatfSite;
        }
    }
}