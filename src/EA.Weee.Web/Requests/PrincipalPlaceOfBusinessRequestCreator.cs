namespace EA.Weee.Web.Requests
{
    using AutoMapper;
    using Base;
    using ViewModels.OrganisationRegistration;
    using Weee.Requests.Organisations;
    using Weee.Requests.Shared;

    public class PrincipalPlaceOfBusinessRequestCreator : RequestCreator<PrincipalPlaceOfBusinessViewModel, AddAddressToOrganisation>, IPrincipalPlaceOfBusinessRequestCreator
    {
        public override AddAddressToOrganisation ViewModelToRequest(PrincipalPlaceOfBusinessViewModel viewModel)
        {
            var addressMap = Mapper.DynamicMap<AddressData>(viewModel);
            return new AddAddressToOrganisation(viewModel.OrganisationId, AddressType.RegisteredorPPBAddress, addressMap);
        }
    }
}