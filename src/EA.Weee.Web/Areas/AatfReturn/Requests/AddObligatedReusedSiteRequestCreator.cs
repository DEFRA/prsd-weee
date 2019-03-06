namespace EA.Weee.Web.Areas.AatfReturn.Requests
{
    using EA.Weee.Requests.AatfReturn.Obligated;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;

    public class AddObligatedReusedSiteRequestCreator : IAddObligatedReusedSiteRequestCreator
    {
        public AddAatfSite ViewModelToRequest(ReusedOffSiteCreateSiteViewModel viewModel)
        {
            var addAatfSite = new AddAatfSite()
            {
                OrganisationId = viewModel.OrganisationId,
                ReturnId = viewModel.ReturnId,
                AatfId = viewModel.AatfId,
                AddressData = viewModel.AddressData
            };

            return addAatfSite;
        }
    }
}