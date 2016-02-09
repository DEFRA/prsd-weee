namespace EA.Weee.RequestHandlers.Organisations
{
    using System;
    using System.Threading.Tasks;
    using Domain;
    using Domain.Organisation;
    using Prsd.Core.Mediator;
    using Requests.Organisations;

    public class UpdateOrganisationDetailsHandler : IRequestHandler<UpdateOrganisationDetails, bool>
    {
        private IOrganisationDetailsDataAccess dataAccess;

        public UpdateOrganisationDetailsHandler(IOrganisationDetailsDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public async Task<bool> HandleAsync(UpdateOrganisationDetails message)
        {
            Organisation organisation = await dataAccess.FetchOrganisationAsync(message.OrganisationData.Id);

            switch (message.OrganisationData.OrganisationType)
            {
                case Core.Organisations.OrganisationType.RegisteredCompany:
                    organisation.UpdateRegisteredCompanyDetails(message.OrganisationData.Name, message.OrganisationData.CompanyRegistrationNumber, message.OrganisationData.TradingName);
                    break;
                case Core.Organisations.OrganisationType.Partnership:
                case Core.Organisations.OrganisationType.SoleTraderOrIndividual:
                    organisation.UpdateSoleTraderOrIndividualDetails(message.OrganisationData.TradingName);
                    break;
            }
            
            Country country = await dataAccess.FetchCountryAsync(message.OrganisationData.BusinessAddress.CountryId);

            Address address = new Address(
                message.OrganisationData.BusinessAddress.Address1,
                message.OrganisationData.BusinessAddress.Address2,
                message.OrganisationData.BusinessAddress.TownOrCity,
                message.OrganisationData.BusinessAddress.CountyOrRegion,
                message.OrganisationData.BusinessAddress.Postcode,
                country,
                message.OrganisationData.BusinessAddress.Telephone,
                message.OrganisationData.BusinessAddress.Email);

            organisation.AddOrUpdateAddress(AddressType.RegisteredOrPPBAddress, address);

            await dataAccess.SaveAsync();

            return true;
        }

        public OrganisationType GetOrganisationType(Core.Organisations.OrganisationType orgType)
        {
            switch (orgType)
            {
                case Core.Organisations.OrganisationType.RegisteredCompany:
                    return OrganisationType.RegisteredCompany;

                case Core.Organisations.OrganisationType.Partnership:
                    return OrganisationType.Partnership;

                case Core.Organisations.OrganisationType.SoleTraderOrIndividual:
                    return OrganisationType.SoleTraderOrIndividual;

                default:
                    throw new ArgumentException(string.Format("Unknown organisation type: {0}", orgType),
                        "orgType");
            }
        }
    }
}
