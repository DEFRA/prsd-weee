namespace EA.Weee.RequestHandlers.Organisations.DirectRegistrants
{
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
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;

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

            var organisationTransaction =
                await organisationTransactionDataAccess.FindIncompleteTransactionForCurrentUserAsync();

            if (organisationTransaction == null)
            {
                throw new InvalidOperationException();
            }

            var organisationTransactionData =
                jsonSerializer.Deserialize<OrganisationTransactionData>(organisationTransaction.OrganisationJson);
            // validate the request

            using (var transaction = transactionAdapter.BeginTransaction())
            {
                Organisation organisation = null;
                ExternalAddressData addressData = null;

                try
                {
                    string brandNames = null;
                    switch (organisationTransactionData.OrganisationType)
                    {
                        case Core.Organisations.ExternalOrganisationType.Partnership:
                            organisation = Organisation.CreateDirectRegistrantCompany(Domain.Organisation.OrganisationType.DirectRegistrantPartnership, organisationTransactionData.PartnershipDetailsViewModel.CompanyName,
                                organisationTransactionData.PartnershipDetailsViewModel.BusinessTradingName, organisationTransactionData.PartnershipDetailsViewModel.CompaniesRegistrationNumber);
                            addressData = organisationTransactionData.PartnershipDetailsViewModel.Address;
                            brandNames = organisationTransactionData.PartnershipDetailsViewModel.EEEBrandNames;
                            break;
                        case Core.Organisations.ExternalOrganisationType.RegisteredCompany:
                            organisation = Organisation.CreateDirectRegistrantCompany(Domain.Organisation.OrganisationType.RegisteredCompany, organisationTransactionData.RegisteredCompanyDetailsViewModel.CompanyName,
                                organisationTransactionData.RegisteredCompanyDetailsViewModel.BusinessTradingName, organisationTransactionData.RegisteredCompanyDetailsViewModel.CompaniesRegistrationNumber);
                            addressData = organisationTransactionData.RegisteredCompanyDetailsViewModel.Address;
                            brandNames = organisationTransactionData.RegisteredCompanyDetailsViewModel.EEEBrandNames;
                            break;
                        case ExternalOrganisationType.SoleTrader:
                            organisation = Organisation.CreateDirectRegistrantCompany(Domain.Organisation.OrganisationType.SoleTraderOrIndividual, organisationTransactionData.SoleTraderDetailsViewModel.CompanyName,
                                organisationTransactionData.SoleTraderDetailsViewModel.BusinessTradingName, organisationTransactionData.SoleTraderDetailsViewModel.CompaniesRegistrationNumber);
                            addressData = organisationTransactionData.SoleTraderDetailsViewModel.Address;
                            brandNames = organisationTransactionData.SoleTraderDetailsViewModel.EEEBrandNames;
                            break;
                        case null:
                            throw new InvalidOperationException();
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    if (addressData == null)
                    {
                        throw new InvalidOperationException("CompleteOrganisationTransactionHandler address null");
                    }

                    var country = await weeeContext.Countries.SingleAsync(c => c.Id == addressData.CountryId);

                    var address = ValueObjectInitializer.CreateAddress(addressData, country);

                    var updatedAddress = await genericDataAccess.Add(address);

                    organisation.AddOrUpdateAddress(AddressType.RegisteredOrPPBAddress, updatedAddress);

                    organisation.CompleteRegistration();

                    organisation = await genericDataAccess.Add<Organisation>(organisation);

                    BrandName brandName = null;
                    if (!string.IsNullOrWhiteSpace(brandNames))
                    {
                        brandName = new BrandName(organisationTransactionData.RegisteredCompanyDetailsViewModel.EEEBrandNames);

                        brandName = await genericDataAccess.Add(brandName);
                    }
                    
                    var directRegistrant = DirectRegistrant.CreateDirectRegistrant(organisation, brandName);

                    await genericDataAccess.Add(directRegistrant);

                    await organisationTransactionDataAccess.CompleteTransactionAsync(organisation);

                    transactionAdapter.Commit(transaction);
                }
                catch
                {
                    transactionAdapter.Rollback(transaction);
                    throw;
                }

                return organisation.Id;
            }
        }
    }
}