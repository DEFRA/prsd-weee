namespace EA.Weee.Web.Requests
{
    using Base;
    using EA.Weee.Core.Organisations;
    using Weee.Requests.Organisations.Create;

    public interface IRegisteredCompanyDetailsRequestCreator : IRequestCreator<RegisteredCompanyDetailsViewModel, CreateRegisteredCompanyRequest>
    {
    }
}