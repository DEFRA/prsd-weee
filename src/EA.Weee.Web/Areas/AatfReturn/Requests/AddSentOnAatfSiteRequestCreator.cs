namespace EA.Weee.Web.Areas.AatfReturn.Requests
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;

    public class AddSentOnAatfSiteRequestCreator : IAddSentOnAatfSiteRequestCreator
    {
        public SentOnAatfSite ViewModelToRequest(SentOnCreateSiteViewModel viewModel)
        {
            if (viewModel.Edit)
            {
                return new EditSentOnAatfSite()
                {
                    SiteAddressData = viewModel.SiteAddressData
                };
            }

            var aatfSite = new AddSentOnAatfSite()
            {
                OrganisationId = viewModel.OrganisationId,
                ReturnId = viewModel.ReturnId,
                AatfId = viewModel.AatfId,
                SiteAddressData = viewModel.SiteAddressData
            };

            return aatfSite;
        }
    }
}