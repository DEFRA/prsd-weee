namespace EA.Weee.RequestHandlers.Organisations.DirectRegistrants
{
    using DataAccess;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.Domain.Producer;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using Mappings;
    using Security;
    using System;
    using System.Threading.Tasks;
    
    internal class AddSignatoryAndCompleteRequestHandler : SubmissionRequestHandlerBase, IRequestHandler<AddSignatoryAndCompleteRequest, bool>
    {
        private readonly WeeeContext weeeContext;
        private readonly ISystemDataDataAccess systemDataAccess;

        public AddSignatoryAndCompleteRequestHandler(IWeeeAuthorization authorization,
            IGenericDataAccess genericDataAccess, WeeeContext weeeContext, ISystemDataDataAccess systemDataAccess, ISmallProducerDataAccess smallProducerDataAccess) : base(authorization, genericDataAccess, systemDataAccess, smallProducerDataAccess)
        {
            this.weeeContext = weeeContext;
            this.systemDataAccess = systemDataAccess;
        }

        public async Task<bool> HandleAsync(AddSignatoryAndCompleteRequest request)
        {
            var currentYearSubmission = await Get(request.DirectRegistrantId);

            var contact = ValueObjectInitializer.CreateContact(request.ContactData);
            currentYearSubmission.CurrentSubmission.AddOrUpdateAppropriateSignatory(contact);

            // Use the actual current date/time for SubmittedDate instead of mixing system year with current time
            currentYearSubmission.CurrentSubmission.SubmittedDate = SystemTime.UtcNow;

            currentYearSubmission.DirectProducerSubmissionStatus = DirectProducerSubmissionStatus.Complete;

            await weeeContext.SaveChangesAsync();

            return true;
        }
    }
}
