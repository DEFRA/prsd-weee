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

    internal class AddSignatoryRequestHandler : SubmissionRequestHandlerBase, IRequestHandler<AddSignatoryRequest, bool>
    {
        private readonly WeeeContext weeeContext;

        public AddSignatoryRequestHandler(IWeeeAuthorization authorization,
            IGenericDataAccess genericDataAccess, WeeeContext weeeContext, ISystemDataDataAccess systemDataAccess) : base(authorization, genericDataAccess, systemDataAccess)
        {
            this.weeeContext = weeeContext;
        }

        public async Task<bool> HandleAsync(AddSignatoryRequest request)
        {
            var currentYearSubmission = await Get(request.DirectRegistrantId);

            var contact = ValueObjectInitializer.CreateContact(request.ContactData);

            currentYearSubmission.CurrentSubmission.AddOrUpdateAppropriateSignatory(contact);

            await weeeContext.SaveChangesAsync();

            return true;
        }
    }
}
