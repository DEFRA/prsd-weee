namespace EA.Weee.Web.Areas.Producer.Mappings.ToRequest
{
    using EA.Weee.Core.Organisations;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using EA.Weee.Web.Requests.Base;

    public class ToEditRepresentedOrganisationDetailsRequestCreator : IRequestCreator<RepresentingCompanyDetailsViewModel, RepresentedOrganisationDetailsRequest>
    {
        public RepresentedOrganisationDetailsRequest ViewModelToRequest(RepresentingCompanyDetailsViewModel viewModel)
        {
            return new RepresentedOrganisationDetailsRequest(viewModel.DirectRegistrantId, viewModel.BusinessTradingName, viewModel.Address);
        }
    }
}