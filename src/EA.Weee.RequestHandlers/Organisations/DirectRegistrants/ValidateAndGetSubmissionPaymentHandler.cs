namespace EA.Weee.RequestHandlers.Organisations.DirectRegistrants
{
    using DataAccess;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain.Producer;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using Security;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    internal class ValidateAndGetSubmissionPaymentHandler : IRequestHandler<ValidateAndGetSubmissionPayment, SubmissionPaymentDetails>
    {
        private readonly WeeeContext weeeContext;
        private readonly IWeeeAuthorization authorization;
        private readonly ISystemDataDataAccess systemDataAccess;
        private readonly IPaymentSessionDataAccess paymentSessionDataAccess;

        public ValidateAndGetSubmissionPaymentHandler(IWeeeAuthorization authorization,
            WeeeContext weeeContext, ISystemDataDataAccess systemDataAccess, IPaymentSessionDataAccess paymentSessionDataAccess)
        {
            this.weeeContext = weeeContext;
            this.paymentSessionDataAccess = paymentSessionDataAccess;
            this.authorization = authorization;
            this.systemDataAccess = systemDataAccess;
        }

        public async Task<SubmissionPaymentDetails> HandleAsync(ValidateAndGetSubmissionPayment request)
        {
            authorization.EnsureCanAccessExternalArea();

            var requestExists = await paymentSessionDataAccess.AnyPaymentTokenAsync(request.PaymentReturnToken);

            var systemTime = await systemDataAccess.GetSystemDateTime();

            if (!requestExists)
            {
                return new SubmissionPaymentDetails()
                {
                    ErrorMessage = $"No payment request exists {request.PaymentReturnToken}"
                };
            }

            // we only care about the most recent session as user should only have one payment process at once.
            var session = await paymentSessionDataAccess.GetCurrentInProgressPayment(request.PaymentReturnToken, request.DirectRegistrantId, systemTime.Year);

            if (session == null)
            {
                return new SubmissionPaymentDetails()
                {
                    ErrorMessage = $"No payment request {request.PaymentReturnToken} exists for user"
                };
            }

            // check user has access to the direct registrant
            authorization.EnsureOrganisationAccess(session.DirectRegistrant.OrganisationId);

            await weeeContext.SaveChangesAsync();

            return new SubmissionPaymentDetails()
            {
                DirectRegistrantId = session.DirectRegistrantId,
                PaymentId = session.PaymentId,
                PaymentReference = session.PaymentReference,
                PaymentSessionId = session.Id
            };
        }
    }
}
