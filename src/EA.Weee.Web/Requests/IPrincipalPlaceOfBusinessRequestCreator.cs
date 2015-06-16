namespace EA.Weee.Web.Requests
{
    using Base;
    using ViewModels.OrganisationRegistration;
    using Weee.Requests.Organisations;

    public interface IPrincipalPlaceOfBusinessRequestCreator : IRequestCreator<PrincipalPlaceOfBusinessViewModel, SaveOrganisationPrincipalPlaceOfBusiness>
    {
    }
}
