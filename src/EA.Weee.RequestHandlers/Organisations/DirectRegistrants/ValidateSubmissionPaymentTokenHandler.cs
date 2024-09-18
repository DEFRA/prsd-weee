namespace EA.Weee.RequestHandlers.Organisations.DirectRegistrants
{
    using DataAccess;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using Security;
    using System.Data.Entity;
    using System.Threading.Tasks;

    internal class ValidateSubmissionPaymentTokenHandler : SubmissionRequestHandlerBase, IRequestHandler<ValidateSubmissionPaymentTokenRequest, bool>
    {
        private readonly WeeeContext weeeContext;

        public ValidateSubmissionPaymentTokenHandler(IWeeeAuthorization authorization,
            IGenericDataAccess genericDataAccess, WeeeContext weeeContext, ISystemDataDataAccess systemDataAccess) : base(authorization, genericDataAccess, systemDataAccess)
        {
            this.weeeContext = weeeContext;
        }

        public async Task<bool> HandleAsync(ValidateSubmissionPaymentTokenRequest request)
        {
            var requestExists = await weeeContext.DirectProducerSubmissions.FirstOrDefaultAsync(c =>
                c.PaymentReturnToken == request.PaymentReturnToken &&
                c.PaymentId == request.PaymentId &&
                c.DirectRegistrantId == request.DirectRegistrantId &&
                c.PaymentFinished == false);

            // Also check the status when it is in place

            return requestExists != null;
        }
    }
}
