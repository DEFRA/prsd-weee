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

    internal class CompleteMigratedOrganisationTransactionHandler : CompleteOrganisationTransactionHandlerBase, IRequestHandler<CompleteMigratedOrganisationTransaction, Guid>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IOrganisationTransactionDataAccess organisationTransactionDataAccess;
        private readonly IJsonSerializer jsonSerializer;
        private readonly IWeeeTransactionAdapter transactionAdapter;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly WeeeContext weeeContext;
        private readonly IUserContext userContext;

        public CompleteMigratedOrganisationTransactionHandler(
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
            this.weeeContext = weeeContext;
            this.userContext = userContext;
        }

        public async Task<Guid> HandleAsync(CompleteMigratedOrganisationTransaction request)
        {
            authorization.EnsureCanAccessExternalArea();

            var organisationTransaction = await organisationTransactionDataAccess.FindIncompleteTransactionForCurrentUserAsync();
            var directRegistrant = await genericDataAccess.GetById<DirectRegistrant>(request.DirectRegistrantId);

            if (organisationTransaction == null)
            {
                throw new InvalidOperationException("Organisation transaction not found.");
            }

            if (directRegistrant == null)
            {
                throw new InvalidOperationException($"Migrated direct registrant not found {request.DirectRegistrantId}");
            }

            var organisationTransactionData = jsonSerializer.Deserialize<OrganisationTransactionData>(organisationTransaction.OrganisationJson);
            using (var transaction = transactionAdapter.BeginTransaction())
            {
                try
                {
                    var organisation = directRegistrant.Organisation;

                    if (!organisation.NpwdMigrated)
                    {
                        throw new InvalidOperationException("Not a NPWD migrated organisation.");
                    }

                    var address = await CreateAndAddAddress(organisationTransactionData, organisation);

                    organisation.AddOrUpdateAddress(AddressType.RegisteredOrPPBAddress, address);
                    organisation.UpdateMigratedOrganisationType(GetOrganisationType(organisationTransactionData));
                    organisation.UpdateDirectRegistrantDetails(organisationTransactionData.OrganisationViewModel.CompanyName,
                        organisationTransactionData.OrganisationViewModel.BusinessTradingName,
                        organisation.CompanyRegistrationNumber);
                    organisation.ToMigrated();

                    var brandName = await CreateAndAddBrandName(organisationTransactionData);
                    var representingCompany = await CreateRepresentingCompany(organisationTransactionData);
                    var contactDetails = CreateContact(organisationTransactionData);
                    var contactAddress = await CreateContactAddress(organisationTransactionData);

                    var additionalCompanyDetails = CreateAdditionalCompanyDetails(organisationTransactionData);

                    if (brandName != null)
                    {
                        directRegistrant.AddOrUpdateBrandName(brandName);
                    }

                    directRegistrant.AddOrUpdateMainContactPerson(contactDetails);
                    directRegistrant.AddOrUpdateAddress(contactAddress);

                    if (representingCompany != null)
                    {
                        directRegistrant.AddOrUpdateAuthorisedRepresentitive(representingCompany);
                    }

                    if (additionalCompanyDetails != null)
                    {
                        directRegistrant.SetAdditionalCompanyDetails(additionalCompanyDetails);
                    }

                    organisation.IsRepresentingCompany = organisationTransactionData.AuthorisedRepresentative == YesNoType.Yes;

                    await organisationTransactionDataAccess.CompleteTransactionAsync(directRegistrant.Organisation);

                    var organisationUser =
                        new OrganisationUser(userContext.UserId, directRegistrant.OrganisationId, UserStatus.Active);

                    await genericDataAccess.Add(organisationUser);

                    transactionAdapter.Commit(transaction);

                    await weeeContext.SaveChangesAsync();

                    return directRegistrant.OrganisationId;
                }
                catch
                {
                    transactionAdapter.Rollback(transaction);
                    throw;
                }
            }
        }

        private static Domain.Organisation.OrganisationType GetOrganisationType(OrganisationTransactionData organisationTransactionData)
        {
            switch (organisationTransactionData.OrganisationType)
            {
                case ExternalOrganisationType.Partnership:
                    return Domain.Organisation.OrganisationType.DirectRegistrantPartnership;
                case ExternalOrganisationType.RegisteredCompany:
                    return Domain.Organisation.OrganisationType.RegisteredCompany;
                case ExternalOrganisationType.SoleTrader:
                    return Domain.Organisation.OrganisationType.SoleTraderOrIndividual;
                case null:
                    throw new InvalidOperationException("Organisation type is null.");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
