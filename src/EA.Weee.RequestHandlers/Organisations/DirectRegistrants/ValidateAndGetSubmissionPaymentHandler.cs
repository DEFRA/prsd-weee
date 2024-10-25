namespace EA.Weee.RequestHandlers.Organisations.DirectRegistrants
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Helpers;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using Security;
    using System;
    using System.Threading.Tasks;

    internal class ValidateAndGetSubmissionPaymentHandler : IRequestHandler<ValidateAndGetSubmissionPayment, SubmissionPaymentDetails>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly ISystemDataDataAccess systemDataAccess;
        private readonly IPaymentSessionDataAccess paymentSessionDataAccess;
        private readonly ISmallProducerDataAccess smallProducerDataAccess;

        public ValidateAndGetSubmissionPaymentHandler(IWeeeAuthorization authorization,
            ISystemDataDataAccess systemDataAccess, IPaymentSessionDataAccess paymentSessionDataAccess, ISmallProducerDataAccess smallProducerDataAccess)
        {
            this.paymentSessionDataAccess = paymentSessionDataAccess;
            this.smallProducerDataAccess = smallProducerDataAccess;
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

            var directRegistrantSubmission =
                await smallProducerDataAccess.GetCurrentDirectRegistrantSubmissionByComplianceYear(
                    request.DirectRegistrantId,
                    systemTime.Year);

            if (directRegistrantSubmission == null)
            {
                throw new InvalidOperationException("No direct registrant submission found");
            }

            // check user has access to the direct registrant
            authorization.EnsureOrganisationAccess(directRegistrantSubmission.DirectRegistrant.OrganisationId);

            // if already finished
            if (directRegistrantSubmission.PaymentFinished)
            {
                return new SubmissionPaymentDetails()
                {
                    DirectRegistrantId = directRegistrantSubmission.DirectRegistrantId,
                    PaymentReference = directRegistrantSubmission.FinalPaymentSession.PaymentReference,
                    PaymentFinished = true,
                    PaymentStatus = directRegistrantSubmission.FinalPaymentSession.Status.ToCoreEnumeration<PaymentStatus>()
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
