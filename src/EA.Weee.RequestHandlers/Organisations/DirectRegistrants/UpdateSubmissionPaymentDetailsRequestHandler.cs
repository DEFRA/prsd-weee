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
        private readonly ISystemDataDataAccess systemDataDataAccess;

        public UpdateSubmissionPaymentDetailsRequestHandler(IWeeeAuthorization authorization,
            IGenericDataAccess genericDataAccess, WeeeContext weeeContext, ISystemDataDataAccess systemDataAccess, ISystemDataDataAccess systemDataDataAccess) : base(authorization, genericDataAccess, systemDataAccess)
        {
            this.weeeContext = weeeContext;
            this.systemDataDataAccess = systemDataDataAccess;
        }

        public async Task<bool> HandleAsync(UpdateSubmissionPaymentDetailsRequest request)
        {
            var currentYearSubmission = await Get(request.DirectRegistrantId);

            var systemDateTime = await systemDataDataAccess.GetSystemDateTime();

            var paymentSession = await weeeContext.PaymentSessions.FirstOrDefaultAsync(p => p.Id == request.PaymentSessionId);

            if (paymentSession == null)
            {
                throw new InvalidOperationException($"Payment session {request.PaymentSessionId} not found");
            }

            paymentSession.Status = request.PaymentStatus.ToDomainEnumeration<PaymentState>();
            currentYearSubmission.FinalPaymentSession = paymentSession;

            await weeeContext.SaveChangesAsync();
  
            return true;
        }
    }
}
