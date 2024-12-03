namespace EA.Weee.RequestHandlers.Organisations.DirectRegistrants
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Organisations.Base;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using Security;
    using System;
    using System.Threading.Tasks;

    internal class ContinueOrganisationRegistrationRequestHandler : IRequestHandler<ContinueOrganisationRegistrationRequest, OrganisationTransactionData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly ISmallProducerDataAccess smallProducerDataAccess;
        private readonly IOrganisationTransactionDataAccess organisationTransactionDataAccess;
        private readonly IJsonSerializer serializer;

        public ContinueOrganisationRegistrationRequestHandler(IWeeeAuthorization authorization, ISmallProducerDataAccess smallProducerDataAccess, IOrganisationTransactionDataAccess organisationTransactionDataAccess, IJsonSerializer serializer)
        {
            this.authorization = authorization;
            this.smallProducerDataAccess = smallProducerDataAccess;
            this.organisationTransactionDataAccess = organisationTransactionDataAccess;
            this.serializer = serializer;
        }

        public async Task<OrganisationTransactionData> HandleAsync(ContinueOrganisationRegistrationRequest request)
        {
            authorization.EnsureCanAccessExternalArea();

            var directRegistrant = await smallProducerDataAccess.GetDirectRegistrantByOrganisationId(request.OrganisationId);
            var organisation = directRegistrant.Organisation;

            if (!organisation.NpwdMigrated)
            {
                throw new InvalidOperationException("Organisation not migrated from NPWD");
            }

            if (organisation.NpwdMigratedComplete)
            {
                throw new InvalidOperationException("Migrated organisation is already complete");
            }

            var organisationTransactionData = new OrganisationTransactionData()
            {
                NpwdMigrated = true,
                DirectRegistrantId = directRegistrant.Id,
                OrganisationViewModel = new OrganisationViewModel()
                {
                    CompaniesRegistrationNumber = organisation.CompanyRegistrationNumber,
                    ProducerRegistrationNumber = directRegistrant.ProducerRegistrationNumber,
                    CompanyName = organisation.Name,
                    BusinessTradingName = organisation.TradingName,
                }
            };

            var organisationJson = serializer.Serialize(organisationTransactionData);

            await organisationTransactionDataAccess.UpdateOrCreateTransactionAsync(organisationJson);

            return organisationTransactionData;
        }
    }
}
