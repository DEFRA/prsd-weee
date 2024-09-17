namespace EA.Weee.RequestHandlers.Organisations.DirectRegistrants
{
    using DataAccess;
    using Domain.Producer;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using Security;
    using System.Data.Entity;
    using System.Threading.Tasks;

    internal class EditRepresentedOrganisationDetailsRequestHandler : SubmissionRequestHandlerBase, IRequestHandler<RepresentedOrganisationDetailsRequest, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly WeeeContext weeeContext;
        private readonly ISystemDataDataAccess systemDataAccess;

        public EditRepresentedOrganisationDetailsRequestHandler(
            IWeeeAuthorization authorization,
            IGenericDataAccess genericDataAccess,
            WeeeContext weeeContext, 
            ISystemDataDataAccess systemDataAccess) : base(authorization, genericDataAccess, systemDataAccess)
        {
            this.authorization = authorization;
            this.genericDataAccess = genericDataAccess;
            this.weeeContext = weeeContext;
            this.systemDataAccess = systemDataAccess;
        }

        public async Task<bool> HandleAsync(RepresentedOrganisationDetailsRequest request)
        {
            var currentYearSubmission = await Get(request.DirectRegistrantId);

            var country = await weeeContext.Countries.SingleAsync(c => c.Id == request.Address.CountryId);

            request.Address.CountryName = country.Name;

            var authorisedRepresentative = await CreateRepresentingCompany(request, currentYearSubmission.DirectRegistrant);

            currentYearSubmission.CurrentSubmission.AddOrUpdateAuthorisedRepresentative(authorisedRepresentative);

            await weeeContext.SaveChangesAsync();

            return true;
        }

        private async Task<AuthorisedRepresentative> CreateRepresentingCompany(RepresentedOrganisationDetailsRequest representedOrganisationDetailsRequest,
            DirectRegistrant directRegistrant)
        {
            AuthorisedRepresentative authorisedRepresentative = null;
            
            var country = await weeeContext.Countries.SingleAsync(c => c.Id == representedOrganisationDetailsRequest.Address.CountryId);

            var producerAddress = new ProducerAddress(
                primaryName: representedOrganisationDetailsRequest.Address.Address1,
                secondaryName: representedOrganisationDetailsRequest.Address.Address2,
                street: representedOrganisationDetailsRequest.Address.Address2 ?? string.Empty,
                town: representedOrganisationDetailsRequest.Address.TownOrCity,
                string.Empty,
                representedOrganisationDetailsRequest.Address.CountyOrRegion ?? string.Empty,
                country,
                representedOrganisationDetailsRequest.Address.Postcode);

            var producerContact = new ProducerContact(string.Empty,
                string.Empty,
                string.Empty,
                representedOrganisationDetailsRequest.Address.Telephone,
                string.Empty,
                string.Empty,
                representedOrganisationDetailsRequest.Address.Email,
                producerAddress);

            authorisedRepresentative = new AuthorisedRepresentative(directRegistrant.AuthorisedRepresentative.OverseasProducerName, representedOrganisationDetailsRequest.BusinessTradingName, producerContact);
            
            return authorisedRepresentative;
        }
    }
}
