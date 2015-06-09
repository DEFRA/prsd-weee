namespace EA.Weee.Web.Requests
{
    using Requests.Base;
    using ViewModels.OrganisationRegistration.Details;
    using Weee.Requests.Organisations.Create;

    public class PartnershipDetailsRequestCreator : RequestCreator<PartnershipDetailsViewModel, CreatePartnershipRequest>, IPartnershipDetailsRequestCreator
    {
        public override CreatePartnershipRequest ViewModelToRequest(PartnershipDetailsViewModel viewModel)
        {
            // Auto mappings
            var request = base.ViewModelToRequest(viewModel);

            // Manual mappings (where names differ)
            request.TradingName = viewModel.BusinessTradingName;

            return request;
        }
    }
}