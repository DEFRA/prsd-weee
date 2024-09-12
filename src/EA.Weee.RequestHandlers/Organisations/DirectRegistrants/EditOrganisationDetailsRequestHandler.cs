namespace EA.Weee.RequestHandlers.Organisations.DirectRegistrants
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.DataAccess;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain.Producer;
    using EA.Weee.RequestHandlers.Mappings;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    internal class EditOrganisationDetailsRequestHandler : IRequestHandler<EditOrganisationDetailsRequest, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly WeeeContext weeeContext;

        public EditOrganisationDetailsRequestHandler(IWeeeAuthorization authorization,
            IGenericDataAccess genericDataAccess, WeeeContext weeeContext)
        {
            this.authorization = authorization;
            this.genericDataAccess = genericDataAccess;
            this.weeeContext = weeeContext;
        }

        public async Task<bool> HandleAsync(EditOrganisationDetailsRequest request)
        {
            authorization.EnsureCanAccessExternalArea();

            var directRegistrant = await genericDataAccess.GetById<DirectRegistrant>(request.DirectRegistrantId);

            authorization.EnsureOrganisationAccess(directRegistrant.OrganisationId);

            var currentYearSubmission = directRegistrant.DirectProducerSubmissions.First(r => r.ComplianceYear == SystemTime.UtcNow.Year);
            
            var country = await weeeContext.Countries.SingleAsync(c => c.Id == request.BusinessAddressData.CountryId);

            request.BusinessAddressData.CountryName = country.Name;

            var address = ValueObjectInitializer.CreateAddress(request.BusinessAddressData, country);

            currentYearSubmission.CurrentSubmission.CompanyName = request.CompanyName;
            currentYearSubmission.CurrentSubmission.TradingName = request.TradingName;
            currentYearSubmission.CurrentSubmission.AddOrUpdateBusinessAddress(address);

            if (!string.IsNullOrWhiteSpace(request.EEEBrandNames))
            {
                var brandNames = new BrandName(request.EEEBrandNames);
                currentYearSubmission.CurrentSubmission.AddOrUpdateBrandName(brandNames);
            }

            await weeeContext.SaveChangesAsync();

            return true;
        }
    }
}
