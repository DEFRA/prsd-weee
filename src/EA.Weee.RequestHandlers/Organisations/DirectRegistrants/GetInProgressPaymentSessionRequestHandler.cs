namespace EA.Weee.RequestHandlers.Organisations.DirectRegistrants
{
    using Core.DirectRegistrant;
    using Domain.Producer;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using Security;
    using System.Threading.Tasks;

    internal class GetInProgressPaymentSessionRequestHandler : IRequestHandler<GetInProgressPaymentSessionRequest, SubmissionPaymentDetails>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly ISystemDataDataAccess systemDataDataAccess;
        private readonly IPaymentSessionDataAccess paymentSessionDataAccess;

        public GetInProgressPaymentSessionRequestHandler(IWeeeAuthorization authorization, 
            IGenericDataAccess genericDataAccess, ISystemDataDataAccess systemDataDataAccess, IPaymentSessionDataAccess paymentSessionDataAccess)
        {
            this.authorization = authorization;
            this.genericDataAccess = genericDataAccess;
            this.systemDataDataAccess = systemDataDataAccess;
            this.paymentSessionDataAccess = paymentSessionDataAccess;
        }

        public async Task<SubmissionPaymentDetails> HandleAsync(GetInProgressPaymentSessionRequest request)
        { 
            authorization.EnsureCanAccessExternalArea();

            var directRegistrant = await genericDataAccess.GetById<DirectRegistrant>(request.DirectRegistrantId);

            authorization.EnsureOrganisationAccess(directRegistrant.OrganisationId);

            var systemTime = await systemDataDataAccess.GetSystemDateTime();

            var paymentSession = await paymentSessionDataAccess.GetCurrentRetryPayment(request.DirectRegistrantId, systemTime.Year);

            if (paymentSession == null)
            {
                return null;
            }

            return new SubmissionPaymentDetails()
            {
                PaymentId = paymentSession.PaymentId,
                PaymentSessionId = paymentSession.Id
            };
        }
    }
}
