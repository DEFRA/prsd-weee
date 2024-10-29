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

        public ValidateAndGetSubmissionPaymentHandler(
            IWeeeAuthorization authorization,
            ISystemDataDataAccess systemDataAccess,
            IPaymentSessionDataAccess paymentSessionDataAccess,
            ISmallProducerDataAccess smallProducerDataAccess)
        {
            this.authorization = authorization;
            this.systemDataAccess = systemDataAccess;
            this.paymentSessionDataAccess = paymentSessionDataAccess;
            this.smallProducerDataAccess = smallProducerDataAccess;
        }

        public async Task<SubmissionPaymentDetails> HandleAsync(ValidateAndGetSubmissionPayment request)
        {
            if (string.IsNullOrEmpty(request.PaymentReturnToken))
            {
                return new SubmissionPaymentDetails
                {
                    ErrorMessage = "Payment return token is required"
                };
            }

            authorization.EnsureCanAccessExternalArea();

            if (!await paymentSessionDataAccess.AnyPaymentTokenAsync(request.PaymentReturnToken))
            {
                return new SubmissionPaymentDetails
                {
                    ErrorMessage = $"No payment request exists {request.PaymentReturnToken}"
                };
            }

            var systemTime = await systemDataAccess.GetSystemDateTime();
            var directRegistrantSubmission = await GetDirectRegistrantSubmission(request.DirectRegistrantId, systemTime.Year);

            authorization.EnsureOrganisationAccess(directRegistrantSubmission.DirectRegistrant.OrganisationId);

            if (directRegistrantSubmission.PaymentFinished)
            {
                return CreateFinishedPaymentDetails(directRegistrantSubmission);
            }

            var session = await paymentSessionDataAccess.GetCurrentPayment(
                request.PaymentReturnToken,
                request.DirectRegistrantId,
                systemTime.Year);

            if (session == null)
            {
                return new SubmissionPaymentDetails
                {
                    ErrorMessage = $"No payment request {request.PaymentReturnToken} exists for user"
                };
            }

            return CreatePaymentDetails(session);
        }

        private async Task<Domain.Producer.DirectProducerSubmission> GetDirectRegistrantSubmission(Guid directRegistrantId, int complianceYear)
        {
            var directRegistrantSubmission = await smallProducerDataAccess
                .GetCurrentDirectRegistrantSubmissionByComplianceYear(directRegistrantId, complianceYear);

            if (directRegistrantSubmission == null)
            {
                throw new InvalidOperationException($"No direct registrant submission found for ID {directRegistrantId} and year {complianceYear}");
            }

            return directRegistrantSubmission;
        }

        private static SubmissionPaymentDetails CreateFinishedPaymentDetails(Domain.Producer.DirectProducerSubmission submission)
        {
            return new SubmissionPaymentDetails
            {
                DirectRegistrantId = submission.DirectRegistrantId,
                PaymentReference = submission.FinalPaymentSession.PaymentReference,
                PaymentFinished = true,
                PaymentStatus = submission.FinalPaymentSession.Status.ToCoreEnumeration<PaymentStatus>()
            };
        }

        private static SubmissionPaymentDetails CreatePaymentDetails(Domain.Producer.PaymentSession session)
        {
            return new SubmissionPaymentDetails
            {
                DirectRegistrantId = session.DirectRegistrantId,
                PaymentId = session.PaymentId,
                PaymentReference = session.PaymentReference,
                PaymentSessionId = session.Id
            };
        }
    }
}