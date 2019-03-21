namespace EA.Weee.Web.Areas.AatfReturn.Requests
{
    using EA.Weee.Requests.AatfReturn.Obligated;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;

    public class ObligatedReusedSiteRequestCreator : IObligatedReusedSiteRequestCreator
    {
        public AatfSite ViewModelToRequest(ReusedOffSiteCreateSiteViewModel viewModel)
        {
            if (viewModel.Edit)
            {
                return new EditAatfSite()
                {
                    AddressData = viewModel.AddressData
                };
            }

            return new AddAatfSite()
            {
                OrganisationId = viewModel.OrganisationId,
                ReturnId = viewModel.ReturnId,
                AatfId = viewModel.AatfId,
                AddressData = viewModel.AddressData
            };
        }
    }
}