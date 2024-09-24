namespace EA.Weee.RequestHandlers.Organisations.DirectRegistrants
{
    using DataAccess;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Helpers;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain.Producer;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using Security;
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;

    internal class UpdateSubmissionPaymentDetailsRequestHandler : SubmissionRequestHandlerBase, IRequestHandler<UpdateSubmissionPaymentDetailsRequest, bool>
    {
        private readonly WeeeContext weeeContext;

        public UpdateSubmissionPaymentDetailsRequestHandler(IWeeeAuthorization authorization,
            IGenericDataAccess genericDataAccess, WeeeContext weeeContext, ISystemDataDataAccess systemDataAccess, ISystemDataDataAccess systemDataDataAccess) : base(authorization, genericDataAccess, systemDataAccess)
        {
            this.weeeContext = weeeContext;
        }

        public async Task<bool> HandleAsync(UpdateSubmissionPaymentDetailsRequest request)
        {
            var currentYearSubmission = await Get(request.DirectRegistrantId);

            var paymentSession = await weeeContext.PaymentSessions.FirstOrDefaultAsync(p => p.Id == request.PaymentSessionId);

            if (paymentSession == null)
            {
                throw new InvalidOperationException($"Payment session {request.PaymentSessionId} not found");
            }

            paymentSession.Status = request.PaymentStatus.ToDomainEnumeration<PaymentState>();
            paymentSession.InFinalState = request.IsFinalState;

            currentYearSubmission.FinalPaymentSession = paymentSession;

            await weeeContext.SaveChangesAsync();
  
            return true;
        }
    }
}
