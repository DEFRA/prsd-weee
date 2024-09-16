namespace EA.Weee.RequestHandlers.Organisations.DirectRegistrants
{
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

    internal class EditContactDetailsRequestHandler : IRequestHandler<EditContactDetailsRequest, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly WeeeContext weeeContext;
        private readonly ISystemDataDataAccess systemDataAccess;

        public EditContactDetailsRequestHandler(IWeeeAuthorization authorization,
            IGenericDataAccess genericDataAccess, WeeeContext weeeContext, ISystemDataDataAccess systemDataAccess)
        {
            this.authorization = authorization;
            this.genericDataAccess = genericDataAccess;
            this.weeeContext = weeeContext;
            this.systemDataAccess = systemDataAccess;
        }

        public async Task<bool> HandleAsync(EditContactDetailsRequest request)
        {
            authorization.EnsureCanAccessExternalArea();

            var directRegistrant = await genericDataAccess.GetById<DirectRegistrant>(request.DirectRegistrantId);

            authorization.EnsureOrganisationAccess(directRegistrant.OrganisationId);

            var systemDateTime = await systemDataAccess.GetSystemDateTime();

            var currentYearSubmission = directRegistrant.DirectProducerSubmissions.First(r => r.ComplianceYear == systemDateTime.Year);
            
            var country = await weeeContext.Countries.SingleAsync(c => c.Id == request.AddressData.CountryId);

            var address = ValueObjectInitializer.CreateAddress(request.AddressData, country);
            var contact = ValueObjectInitializer.CreateContact(request.ContactData);

            currentYearSubmission.CurrentSubmission.AddOrUpdateContactAddress(address);
            currentYearSubmission.CurrentSubmission.AddOrUpdateContact(contact);

            await weeeContext.SaveChangesAsync();

            return true;
        }
    }
}
