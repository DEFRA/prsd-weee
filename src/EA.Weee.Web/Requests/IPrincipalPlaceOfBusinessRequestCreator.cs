namespace EA.Weee.Web.Requests
{
    using Base;
    using ViewModels.OrganisationRegistration;
    using ViewModels.OrganisationRegistration.PrincipalPlaceOfBusiness;
    using Weee.Requests.Organisations;

    public interface IPrincipalPlaceOfBusinessRequestCreator : IRequestCreator<PrincipalPlaceOfBusinessViewModel, SaveOrganisationPrincipalPlaceOfBusiness>
    {
    }
}
