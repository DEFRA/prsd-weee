namespace EA.Weee.RequestHandlers.Organisations.DirectRegistrants
{
    using Core.Helpers;
    using DataAccess;
    using Domain;
    using Domain.Organisation;
    using Domain.Producer;
    using Domain.User;
    using EA.Prsd.Core.Domain;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Organisations;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using Security;
    using System;
    using System.Threading.Tasks;

    internal class CompleteOrganisationTransactionHandler : CompleteOrganisationTransactionHandlerBase, IRequestHandler<CompleteOrganisationTransaction, Guid>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IOrganisationTransactionDataAccess organisationTransactionDataAccess;
        private readonly IJsonSerializer jsonSerializer;
        private readonly IWeeeTransactionAdapter transactionAdapter;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IUserContext userContext;

        public CompleteOrganisationTransactionHandler(
            IWeeeAuthorization authorization,
            IOrganisationTransactionDataAccess organisationTransactionDataAccess,
            IJsonSerializer jsonSerializer,
            IWeeeTransactionAdapter transactionAdapter,
            IGenericDataAccess genericDataAccess,
            WeeeContext weeeContext, 
            IUserContext userContext) : base(genericDataAccess, weeeContext)
        {
            this.authorization = authorization;
            this.organisationTransactionDataAccess = organisationTransactionDataAccess;
            this.jsonSerializer = jsonSerializer;
            this.transactionAdapter = transactionAdapter;
            this.genericDataAccess = genericDataAccess;
            this.userContext = userContext;
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

                    var additionalCompanyDetails = CreateAdditionalCompanyDetails(organisationTransactionData);

                    var producerRegistrationNumber = organisationTransactionData.OrganisationViewModel.ProducerRegistrationNumber;

                    var directRegistrant = DirectRegistrant.CreateDirectRegistrant(organisation, brandName, contactDetails, contactAddress, representingCompany, additionalCompanyDetails, producerRegistrationNumber);

                    organisation.IsRepresentingCompany = organisationTransactionData.AuthorisedRepresentative == YesNoType.Yes;
                    
                    directRegistrant = await genericDataAccess.Add(directRegistrant);

                    await organisationTransactionDataAccess.CompleteTransactionAsync(directRegistrant.Organisation);

                    var organisationUser =
                        new OrganisationUser(userContext.UserId, directRegistrant.Organisation.Id, UserStatus.Active);
                    
                    await genericDataAccess.Add(organisationUser);

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
    }
}
