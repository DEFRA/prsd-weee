namespace EA.Weee.RequestHandlers.Organisations.DirectRegistrants
{
    using DataAccess;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using Mappings;
    using Security;
    using System.Data.Entity;
    using System.Threading.Tasks;

    internal class EditContactDetailsRequestHandler : SubmissionRequestHandlerBase, IRequestHandler<EditContactDetailsRequest, bool>
    {
        private readonly WeeeContext weeeContext;

        public EditContactDetailsRequestHandler(IWeeeAuthorization authorization,
            IGenericDataAccess genericDataAccess, WeeeContext weeeContext, ISystemDataDataAccess systemDataAccess) : base(authorization, genericDataAccess, systemDataAccess)
        {
            this.weeeContext = weeeContext;
        }

        public async Task<bool> HandleAsync(EditContactDetailsRequest request)
        {
            var currentYearSubmission = await Get(request.DirectRegistrantId);
            
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
