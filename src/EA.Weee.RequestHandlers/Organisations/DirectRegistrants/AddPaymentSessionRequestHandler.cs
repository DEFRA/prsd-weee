namespace EA.Weee.RequestHandlers.Organisations.DirectRegistrants
{
    using DataAccess;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Domain;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain.Producer;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using Security;
    using System;
    using System.Threading.Tasks;

    internal class AddPaymentSessionRequestHandler : SubmissionRequestHandlerBase, IRequestHandler<AddPaymentSessionRequest, bool>
    {
        private readonly WeeeContext weeeContext;
        private readonly IUserContext userContext;

        public AddPaymentSessionRequestHandler(IWeeeAuthorization authorization,
            IGenericDataAccess genericDataAccess, WeeeContext weeeContext, ISystemDataDataAccess systemDataAccess, ISystemDataDataAccess systemDataDataAccess, IUserContext userContext) : base(authorization, genericDataAccess, systemDataAccess)
        {
            this.weeeContext = weeeContext;
            this.userContext = userContext;
        }

        public async Task<bool> HandleAsync(AddPaymentSessionRequest request)
        {
            var currentYearSubmission = await Get(request.DirectRegistrantId);

            if (currentYearSubmission.FinalPaymentSession != null)
            {
                throw new InvalidOperationException($"Payment already finalised for submission {currentYearSubmission.Id}");
            }

            var paymentSession = new PaymentSession(userContext.UserId.ToString(),
                request.Amount,
                SystemTime.UtcNow,
                currentYearSubmission,
                currentYearSubmission.DirectRegistrant,
                request.PaymentReturnToken,
                request.PaymentId,
                request.PaymentReference);

            weeeContext.PaymentSessions.Add(paymentSession);

            await weeeContext.SaveChangesAsync(); 
  
            return true;
        }
    }
}
