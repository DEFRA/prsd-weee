namespace EA.Weee.RequestHandlers.Organisations.DirectRegistrants
{
    using DataAccess;
    using EA.Prsd.Core.Domain;
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
        private readonly IUserContext userContext;
        private readonly IWeeeAuthorization authorization;
        private readonly ISystemDataDataAccess systemDataAccess;

        public ValidateAndGetSubmissionPaymentHandler(IWeeeAuthorization authorization,
            WeeeContext weeeContext, ISystemDataDataAccess systemDataAccess, IUserContext userContext)
        {
            this.weeeContext = weeeContext;
            this.userContext = userContext;
            this.authorization = authorization;
            this.systemDataAccess = systemDataAccess;
        }

        public async Task<SubmissionPaymentDetails> HandleAsync(ValidateAndGetSubmissionPayment request)
        {
            authorization.CheckCanAccessExternalArea();

            var requestExists = await weeeContext.PaymentSessions.AnyAsync(c =>
                c.PaymentReturnToken == request.PaymentReturnToken);

            var systemTime = await systemDataAccess.GetSystemDateTime();

            if (!requestExists)
            {
                return new SubmissionPaymentDetails()
                {
                    ErrorMessage = $"No payment request exists {request.PaymentReturnToken}"
                };
            }

            // we only care about the most recent session that may have been started
            var session = await weeeContext.PaymentSessions.Where(c =>
                    c.PaymentReturnToken == request.PaymentReturnToken &&
                    c.UserId.ToString() == userContext.UserId.ToString() &&
                    c.Status.Value == PaymentState.New.Value &&
                    c.DirectProducerSubmission.ComplianceYear == systemTime.Year).OrderByDescending(p => p.CreatedAt)
                .Include(paymentSession => paymentSession.DirectRegistrant).FirstOrDefaultAsync();

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
