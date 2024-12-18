namespace EA.Weee.RequestHandlers.Organisations.DirectRegistrants
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Organisations.Base;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.Domain.Producer;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using Security;
    using System;
    using System.Threading.Tasks;

    internal class ContinueOrganisationRegistrationRequestHandler
        : IRequestHandler<ContinueOrganisationRegistrationRequest, OrganisationTransactionData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly ISmallProducerDataAccess smallProducerDataAccess;
        private readonly IOrganisationTransactionDataAccess organisationTransactionDataAccess;
        private readonly IJsonSerializer serializer;

        public ContinueOrganisationRegistrationRequestHandler(
            IWeeeAuthorization authorization,
            ISmallProducerDataAccess smallProducerDataAccess,
            IOrganisationTransactionDataAccess organisationTransactionDataAccess,
            IJsonSerializer serializer)
        {
            this.authorization = authorization;
            this.smallProducerDataAccess = smallProducerDataAccess;
            this.organisationTransactionDataAccess = organisationTransactionDataAccess;
            this.serializer = serializer;
        }

        public async Task<OrganisationTransactionData> HandleAsync(
            ContinueOrganisationRegistrationRequest request)
        {
            authorization.EnsureCanAccessExternalArea();

            var (directRegistrant, organisation) = await ValidateAndGetOrganisation(request.OrganisationId);
            var existingTransaction = await GetExistingTransaction();

            var transactionData = existingTransaction != null
                ? UpdateExistingTransaction(existingTransaction, directRegistrant, organisation)
                : CreateNewTransaction(directRegistrant, organisation);

            await SaveTransaction(transactionData);

            return transactionData;
        }

        private async Task<(DirectRegistrant directRegistrant, Organisation organisation)>
            ValidateAndGetOrganisation(Guid organisationId)
        {
            var directRegistrant = await smallProducerDataAccess
                .GetDirectRegistrantByOrganisationId(organisationId);
            var organisation = directRegistrant.Organisation;

            if (!organisation.NpwdMigrated)
            {
                throw new InvalidOperationException("Organisation not migrated from NPWD");
            }

            if (organisation.NpwdMigratedComplete)
            {
                throw new InvalidOperationException("Migrated organisation is already complete");
            }

            return (directRegistrant, organisation);
        }

        private async Task<OrganisationTransaction> GetExistingTransaction()
        {
            return await organisationTransactionDataAccess.FindIncompleteTransactionForCurrentUserAsync();
        }

        private OrganisationTransactionData UpdateExistingTransaction(
            OrganisationTransaction existingTransaction,
            DirectRegistrant directRegistrant,
            Organisation organisation)
        {
            var data = serializer.Deserialize<OrganisationTransactionData>(
                existingTransaction.OrganisationJson);

            data.NpwdMigrated = true;
            data.DirectRegistrantId = directRegistrant.Id;

            if (data.OrganisationViewModel != null)
            {
                data.OrganisationViewModel.CompaniesRegistrationNumber = organisation.CompanyRegistrationNumber;
                data.OrganisationViewModel.ProducerRegistrationNumber = directRegistrant.ProducerRegistrationNumber;
                data.OrganisationViewModel.CompanyName = organisation.Name;
                data.OrganisationViewModel.BusinessTradingName = organisation.TradingName;
            }
            else
            {
                data.OrganisationViewModel = CreateOrganisationViewModel(organisation, directRegistrant);
            }
            return data;
        }

        private static OrganisationTransactionData CreateNewTransaction(
            DirectRegistrant directRegistrant,
            Organisation organisation)
        {
            return new OrganisationTransactionData
            {
                NpwdMigrated = true,
                DirectRegistrantId = directRegistrant.Id,
                OrganisationViewModel = CreateOrganisationViewModel(organisation, directRegistrant)
            };
        }

        private static OrganisationViewModel CreateOrganisationViewModel(
            Organisation organisation,
            DirectRegistrant directRegistrant)
        {
            return new OrganisationViewModel
            {
                CompaniesRegistrationNumber = organisation.CompanyRegistrationNumber,
                ProducerRegistrationNumber = directRegistrant.ProducerRegistrationNumber,
                CompanyName = organisation.Name,
                BusinessTradingName = organisation.TradingName
            };
        }

        private async Task SaveTransaction(OrganisationTransactionData transactionData)
        {
            var organisationJson = serializer.Serialize(transactionData);
            await organisationTransactionDataAccess.UpdateOrCreateTransactionAsync(organisationJson);
        }
    }
}