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
    using System.Threading.Tasks;
    using EA.Weee.Core.DirectRegistrant;

    internal class UpdateSubmissionPaymentDetailsRequestHandler : SubmissionRequestHandlerBase, IRequestHandler<UpdateSubmissionPaymentDetailsRequest, bool>
    {
        private readonly WeeeContext weeeContext;
        private readonly IPaymentSessionDataAccess paymentSessionDataAccess;

        public UpdateSubmissionPaymentDetailsRequestHandler(IWeeeAuthorization authorization,
            IGenericDataAccess genericDataAccess, WeeeContext weeeContext, ISystemDataDataAccess systemDataAccess, IPaymentSessionDataAccess paymentSessionDataAccess, ISmallProducerDataAccess smallProducerDataAccess) : base(authorization, genericDataAccess, systemDataAccess, smallProducerDataAccess)
        {
            this.weeeContext = weeeContext;
            this.paymentSessionDataAccess = paymentSessionDataAccess;
        }

        public async Task<bool> HandleAsync(UpdateSubmissionPaymentDetailsRequest request)
        {
            var currentYearSubmission = await Get(request.DirectRegistrantId);

            var paymentSession = await paymentSessionDataAccess.GetByIdAsync(request.PaymentSessionId);

            if (paymentSession == null)
            {
                throw new InvalidOperationException($"Payment session {request.PaymentSessionId} not found");
            }

            paymentSession.Status = request.PaymentStatus.ToDomainEnumeration<PaymentState>();
            paymentSession.InFinalState = request.IsFinalState;

            if (request.IsFinalState)
            {
                currentYearSubmission.FinalPaymentSession = paymentSession;
                if (request.PaymentStatus == PaymentStatus.Success)
                {
                    currentYearSubmission.PaymentFinished = true;
                }
            }

            await weeeContext.SaveChangesAsync();
  
            return true;
        }
    }
}
