namespace EA.Weee.RequestHandlers.Organisations.DirectRegistrants
{
    using DataAccess;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using Security;
    using System.Data.Entity.Validation;
    using System.Threading.Tasks;

    internal class UpdateSubmissionPaymentDetailsRequestHandler : SubmissionRequestHandlerBase, IRequestHandler<UpdateSubmissionPaymentDetailsRequest, bool>
    {
        private readonly WeeeContext weeeContext;

        public UpdateSubmissionPaymentDetailsRequestHandler(IWeeeAuthorization authorization,
            IGenericDataAccess genericDataAccess, WeeeContext weeeContext, ISystemDataDataAccess systemDataAccess) : base(authorization, genericDataAccess, systemDataAccess)
        {
            this.weeeContext = weeeContext;
        }

        public async Task<bool> HandleAsync(UpdateSubmissionPaymentDetailsRequest request)
        {
            var currentYearSubmission = await Get(request.DirectRegistrantId);
            
            currentYearSubmission.SetPaymentInformation(request.PaymentReference, request.PaymentReturnToken, request.PaymentId);

            try
            {
                await weeeContext.SaveChangesAsync();
            }
            catch (DbEntityValidationException ex)
            {
                int i = 10;
            }

            return true;
        }
    }
}
