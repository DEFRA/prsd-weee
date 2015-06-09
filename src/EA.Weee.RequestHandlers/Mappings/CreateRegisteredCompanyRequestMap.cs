namespace EA.Weee.RequestHandlers.Mappings
{
    using Domain;
    using Prsd.Core.Mapper;
    using Requests.Organisations.Create;

    public class CreateRegisteredCompanyRequestMap : IMap<CreateRegisteredCompanyRequest, Organisation>
    {
        public Organisation Map(CreateRegisteredCompanyRequest source)
        {
            return new Organisation(OrganisationType.RegisteredCompany, OrganisationStatus.Incomplete)
            {
                Name = source.BusinessName,
                TradingName = source.TradingName,
                CompanyRegistrationNumber = source.CompanyRegistrationNumber
            }; 
        }
    }
}
