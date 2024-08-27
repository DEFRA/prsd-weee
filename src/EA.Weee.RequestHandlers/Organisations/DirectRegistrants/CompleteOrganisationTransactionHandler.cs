﻿namespace EA.Weee.RequestHandlers.Organisations.DirectRegistrants
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Core.Organisations;
    using EA.Weee.DataAccess;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.RequestHandlers.Mappings;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;

    internal class CompleteOrganisationTransactionHandler : IRequestHandler<CompleteOrganisationTransaction, bool>
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

        public async Task<bool> HandleAsync(CompleteOrganisationTransaction request)
        {
            authorization.EnsureCanAccessExternalArea();

            var organisationJson = jsonSerializer.Serialize(request.OrganisationTransactionData);
            
            // validate the request

            using (var transaction = transactionAdapter.BeginTransaction())
            {
                Organisation organisation = null;
                ExternalAddressData addressData = null;

                try
                {
                    switch (request.OrganisationTransactionData.OrganisationType)
                    {
                        case Core.Organisations.ExternalOrganisationType.Partnership:
                            organisation = Organisation.CreatePartnership(request.OrganisationTransactionData.PartnershipDetailsViewModel.BusinessTradingName);
                            addressData = request.OrganisationTransactionData.PartnershipDetailsViewModel.Address;
                            break;
                        case Core.Organisations.ExternalOrganisationType.RegisteredCompany:
                            organisation = Organisation.CreateRegisteredCompany(request.OrganisationTransactionData.RegisteredCompanyDetailsViewModel.CompanyName,
                                request.OrganisationTransactionData.RegisteredCompanyDetailsViewModel.CompaniesRegistrationNumber, request.OrganisationTransactionData.RegisteredCompanyDetailsViewModel.BusinessTradingName);
                            addressData = request.OrganisationTransactionData.RegisteredCompanyDetailsViewModel.Address;
                            break;
                        case Core.Organisations.ExternalOrganisationType.SoleTrader:
                            organisation = Organisation.CreateSoleTrader(request.OrganisationTransactionData.RegisteredCompanyDetailsViewModel.CompanyName, request.OrganisationTransactionData.RegisteredCompanyDetailsViewModel.BusinessTradingName);
                            addressData = request.OrganisationTransactionData.SoleTraderDetailsViewModel.Address;
                            break;
                        default:
                            break;
                    }

                    if (addressData == null)
                    {
                        throw new InvalidOperationException();
                    }

                    var country = await weeeContext.Countries.SingleAsync(c => c.Id == addressData.CountryId);

                    Address address = ValueObjectInitializer.CreateAddress(addressData, country);

                    //await dataAccess.Add<Address>(address);

                    //organisation.AddOrUpdateAddress(AddressType.RegisteredOrPPBAddress, address);

                    //var result = await dataAccess.Add<Organisation>(organisation);

                    await organisationTransactionDataAccess.CompleteTransactionAsync(organisationJson);

                    transactionAdapter.Commit(transaction);
                }
                catch
                {
                    transactionAdapter.Rollback(transaction);
                    throw;
                }

                //return note.Id;
            }

            return true;
        }
    }
}