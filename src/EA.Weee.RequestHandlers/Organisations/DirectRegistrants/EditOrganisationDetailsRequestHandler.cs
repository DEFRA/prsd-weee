namespace EA.Weee.RequestHandlers.Organisations.DirectRegistrants
{
    using DataAccess;
    using Domain.Producer;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using Mappings;
    using Security;
    using System.Data.Entity;
    using System.Threading.Tasks;

    internal class EditOrganisationDetailsRequestHandler : EditSubmissionRequestHandlerBase, IRequestHandler<EditOrganisationDetailsRequest, bool>
    {
        private readonly WeeeContext weeeContext;

        public EditOrganisationDetailsRequestHandler(IWeeeAuthorization authorization,
            IGenericDataAccess genericDataAccess, WeeeContext weeeContext, ISystemDataDataAccess systemDataDataAccess) : base(authorization, genericDataAccess, systemDataDataAccess)
        {
            this.weeeContext = weeeContext;
        }

        public async Task<bool> HandleAsync(EditOrganisationDetailsRequest request)
        {
            var currentYearSubmission = await Get(request.DirectRegistrantId);

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
