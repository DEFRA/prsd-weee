namespace EA.Weee.Web.Requests
{
    using Base;
    using ViewModels.OrganisationRegistration.Details;
    using Weee.Requests.Organisations.Create;

    public interface IPartnershipDetailsRequestCreator : IRequestCreator<PartnershipDetailsViewModel, CreatePartnershipRequest>
    {
    }
}