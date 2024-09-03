namespace EA.Weee.RequestHandlers.Organisations.DirectRegistrants
{
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Core.Organisations;
    using EA.Weee.DataAccess;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.Domain.Producer;
    using EA.Weee.RequestHandlers.Mappings;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Organisations.DirectRegistrant;

    internal class CompleteOrganisationTransactionHandler : IRequestHandler<CompleteOrganisationTransaction, Guid>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IOrganisationTransactionDataAccess organisationTransactionDataAccess;
        private readonly IJsonSerializer jsonSerializer;
        private readonly IWeeeTransactionAdapter transactionAdapter;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly WeeeContext weeeContext;

        public CompleteOrganisationTransactionHandler(
            IWeeeAuthorization authorization,
            IOrganisationTransactionDataAccess organisationTransactionDataAccess,
            IJsonSerializer jsonSerializer,
            IWeeeTransactionAdapter transactionAdapter,
            IGenericDataAccess genericDataAccess,
            WeeeContext weeeContext)
        {
            this.authorization = authorization;
            this.organisationTransactionDataAccess = organisationTransactionDataAccess;
            this.jsonSerializer = jsonSerializer;
            this.transactionAdapter = transactionAdapter;
            this.genericDataAccess = genericDataAccess;
            this.weeeContext = weeeContext;
        }

        public async Task<Guid> HandleAsync(CompleteOrganisationTransaction request)
        {
            authorization.EnsureCanAccessExternalArea();

            var organisationTransaction = await organisationTransactionDataAccess.FindIncompleteTransactionForCurrentUserAsync();

            if (organisationTransaction == null)
            {
                throw new InvalidOperationException("Organisation transaction not found.");
            }

            var organisationTransactionData = jsonSerializer.Deserialize<OrganisationTransactionData>(organisationTransaction.OrganisationJson);
            using (var transaction = transactionAdapter.BeginTransaction())
            {
                try
                {
                    var organisation = CreateOrganisation(organisationTransactionData);
                    var address = await CreateAndAddAddress(organisationTransactionData, organisation);

                    organisation.AddOrUpdateAddress(AddressType.RegisteredOrPPBAddress, address);
                    organisation.CompleteRegistration();

                    var brandName = await CreateAndAddBrandName(organisationTransactionData);
                    var representingCompany = await CreateRepresentingCompany(organisationTransactionData);

                    var contactDetails = CreateContact(organisationTransactionData);
                    var contactAddress = await CreateContactAddress(organisationTransactionData);

                    var directRegistrant = DirectRegistrant.CreateDirectRegistrant(organisation, brandName, contactDetails, contactAddress, representingCompany);
                    directRegistrant = await genericDataAccess.Add(directRegistrant);

                    await organisationTransactionDataAccess.CompleteTransactionAsync(directRegistrant.Organisation);
                    transactionAdapter.Commit(transaction);

                    return directRegistrant.Organisation.Id;
                }
                catch
                {
                    transactionAdapter.Rollback(transaction);
                    throw;
                }
            }
        }

        private async Task<AuthorisedRepresentative> CreateRepresentingCompany(OrganisationTransactionData organisationTransactionData)
        {
            AuthorisedRepresentative authorisedRepresentative = null;
            if (organisationTransactionData.AuthorisedRepresentative == YesNoType.Yes)
            {
                var country = await weeeContext.Countries.SingleAsync(c => c.Id == organisationTransactionData.RepresentingCompanyDetailsViewModel.Address.CountryId);

                var producerAddress = new ProducerAddress(
                    organisationTransactionData.RepresentingCompanyDetailsViewModel.Address.Address1,
                    string.Empty,
                    organisationTransactionData.RepresentingCompanyDetailsViewModel.Address.Address2,
                    organisationTransactionData.RepresentingCompanyDetailsViewModel.Address.TownOrCity,
                    string.Empty,
                    organisationTransactionData.RepresentingCompanyDetailsViewModel.Address.CountyOrRegion,
                    country,
                    organisationTransactionData.RepresentingCompanyDetailsViewModel.Address.Postcode);

                var producerContact = new ProducerContact(string.Empty,
                    string.Empty,
                    string.Empty,
                    organisationTransactionData.RepresentingCompanyDetailsViewModel.Address.Telephone,
                    string.Empty,
                    string.Empty,
                    organisationTransactionData.RepresentingCompanyDetailsViewModel.Address.Email,
                    producerAddress);

                authorisedRepresentative = new AuthorisedRepresentative(
                    organisationTransactionData.RepresentingCompanyDetailsViewModel.CompanyName,
                    organisationTransactionData.RepresentingCompanyDetailsViewModel.BusinessTradingName,
                    producerContact);
            }

            return authorisedRepresentative;
        }

        private Contact CreateContact(OrganisationTransactionData organisationTransactionData)
        {
            var contactDetails = new Contact(organisationTransactionData.ContactDetailsViewModel.FirstName, organisationTransactionData.ContactDetailsViewModel.LastName,
                                                organisationTransactionData.ContactDetailsViewModel.Position);
            return contactDetails;
        }

        private async Task<Address> CreateContactAddress(OrganisationTransactionData organisationTransactionData)
        {
            var country = await weeeContext.Countries.SingleAsync(c => c.Id == organisationTransactionData.ContactDetailsViewModel.AddressData.CountryId);
            var contactAddress = new Address(organisationTransactionData.ContactDetailsViewModel.AddressData.Address1,
                                        organisationTransactionData.ContactDetailsViewModel.AddressData.Address2,
                                        organisationTransactionData.ContactDetailsViewModel.AddressData.TownOrCity,
                                        organisationTransactionData.ContactDetailsViewModel.AddressData.CountyOrRegion,
                                        organisationTransactionData.ContactDetailsViewModel.AddressData.Postcode,
                                        country,
                                        organisationTransactionData.ContactDetailsViewModel.AddressData.Telephone,
                                        organisationTransactionData.ContactDetailsViewModel.AddressData.Email);
            return contactAddress;
        }

        private Organisation CreateOrganisation(OrganisationTransactionData organisationTransactionData)
        {
            switch (organisationTransactionData.OrganisationType)
            {
                case ExternalOrganisationType.Partnership:
                    return Organisation.CreateDirectRegistrantCompany(
                        EA.Weee.Domain.Organisation.OrganisationType.DirectRegistrantPartnership,
                        organisationTransactionData.OrganisationViewModel.CompanyName,
                        organisationTransactionData.OrganisationViewModel.BusinessTradingName,
                        organisationTransactionData.OrganisationViewModel.CompaniesRegistrationNumber);
                case ExternalOrganisationType.RegisteredCompany:
                    return Organisation.CreateDirectRegistrantCompany(
                        EA.Weee.Domain.Organisation.OrganisationType.RegisteredCompany,
                        organisationTransactionData.OrganisationViewModel.CompanyName,
                        organisationTransactionData.OrganisationViewModel.BusinessTradingName,
                        organisationTransactionData.OrganisationViewModel.CompaniesRegistrationNumber);
                case ExternalOrganisationType.SoleTrader:
                    return Organisation.CreateDirectRegistrantCompany(
                        EA.Weee.Domain.Organisation.OrganisationType.SoleTraderOrIndividual,
                        organisationTransactionData.OrganisationViewModel.CompanyName,
                        organisationTransactionData.OrganisationViewModel.BusinessTradingName,
                        organisationTransactionData.OrganisationViewModel.CompaniesRegistrationNumber);

                case null:
                    throw new InvalidOperationException("Organisation type is null.");

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async Task<Address> CreateAndAddAddress(OrganisationTransactionData organisationTransactionData, Organisation organisation)
        {
            var addressData = organisationTransactionData.OrganisationViewModel.Address;
            if (addressData == null)
            {
                throw new InvalidOperationException("Address data is null.");
            }

            var country = await weeeContext.Countries.SingleAsync(c => c.Id == addressData.CountryId);
            var address = ValueObjectInitializer.CreateAddress(addressData, country);

            var updatedAddress = await genericDataAccess.Add(address);

            return updatedAddress;
        }

        private async Task<BrandName> CreateAndAddBrandName(OrganisationTransactionData organisationTransactionData)
        {
            var brandNames = organisationTransactionData.OrganisationViewModel.EEEBrandNames;
            if (string.IsNullOrWhiteSpace(brandNames))
            {
                return null;
            }

            var brandName = new BrandName(brandNames);
            return await genericDataAccess.Add(brandName);
        }
    }
}
