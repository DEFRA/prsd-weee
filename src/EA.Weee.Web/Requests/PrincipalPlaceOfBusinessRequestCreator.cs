namespace EA.Weee.Web.Requests
{
    using AutoMapper;
    using Base;
    using ViewModels.OrganisationRegistration;
    using ViewModels.OrganisationRegistration.PrincipalPlaceOfBusiness;
    using Weee.Requests.Organisations;
    using Weee.Requests.Shared;

    public class PrincipalPlaceOfBusinessRequestCreator : RequestCreator<PrincipalPlaceOfBusinessViewModel, SaveOrganisationPrincipalPlaceOfBusiness>, IPrincipalPlaceOfBusinessRequestCreator
    {
        public override SaveOrganisationPrincipalPlaceOfBusiness ViewModelToRequest(PrincipalPlaceOfBusinessViewModel viewModel)
        {
            // TODO: Implement
            return base.ViewModelToRequest(viewModel);
        }
    }
}