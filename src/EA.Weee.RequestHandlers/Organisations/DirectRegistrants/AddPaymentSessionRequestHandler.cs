namespace EA.Weee.RequestHandlers.Organisations.DirectRegistrants
{
    using DataAccess;
    using EA.Prsd.Core.Domain;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain.Producer;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using Security;
    using System.Data.Entity.Validation;
    using System.Threading.Tasks;

    internal class AddPaymentSessionRequestHandler : SubmissionRequestHandlerBase, IRequestHandler<AddPaymentSessionRequest, bool>
    {
        private readonly WeeeContext weeeContext;
        private readonly ISystemDataDataAccess systemDataDataAccess;
        private readonly IUserContext userContext;

        public AddPaymentSessionRequestHandler(IWeeeAuthorization authorization,
            IGenericDataAccess genericDataAccess, WeeeContext weeeContext, ISystemDataDataAccess systemDataAccess, ISystemDataDataAccess systemDataDataAccess, IUserContext userContext) : base(authorization, genericDataAccess, systemDataAccess)
        {
            this.weeeContext = weeeContext;
            this.systemDataDataAccess = systemDataDataAccess;
            this.userContext = userContext;
        }

        public async Task<bool> HandleAsync(AddPaymentSessionRequest request)
        {
            var currentYearSubmission = await Get(request.DirectRegistrantId);
            var systemDateTime = await systemDataDataAccess.GetSystemDateTime();

            // Any further validation to this? TBD
            var paymentSession = new PaymentSession(userContext.UserId.ToString(),
                request.Amount,
                systemDateTime.Date,
                currentYearSubmission,
                currentYearSubmission.DirectRegistrant,
                request.PaymentReturnToken,
                request.PaymentId,
                request.PaymentReference);

            weeeContext.PaymentSessions.Add(paymentSession);

            try
            {
                await weeeContext.SaveChangesAsync();
            }
            catch (DbEntityValidationException ex)
            {
                int i = 10;
            }
  
            return true;
        }
    }
}
