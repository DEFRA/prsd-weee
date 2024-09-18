﻿namespace EA.Weee.RequestHandlers.Organisations.DirectRegistrants
{
    using DataAccess;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using Security;
    using System.Data.Entity.Validation;
    using System.Threading.Tasks;

    internal class ValidateAndUpdateSubmissionPaymentResultRequestHandler : SubmissionRequestHandlerBase, IRequestHandler<ValidateAndUpdateSubmissionPaymentResultRequest, bool>
    {
        private readonly WeeeContext weeeContext;
        private readonly ISystemDataDataAccess systemDataDataAccess;

        public ValidateAndUpdateSubmissionPaymentResultRequestHandler(IWeeeAuthorization authorization,
            IGenericDataAccess genericDataAccess, WeeeContext weeeContext, ISystemDataDataAccess systemDataAccess, ISystemDataDataAccess systemDataDataAccess) : base(authorization, genericDataAccess, systemDataAccess)
        {
            this.weeeContext = weeeContext;
            this.systemDataDataAccess = systemDataDataAccess;
        }

        public async Task<bool> HandleAsync(ValidateAndUpdateSubmissionPaymentResultRequest request)
        {
            var currentYearSubmission = await Get(request.DirectRegistrantId);

            var systemDateTime = await systemDataDataAccess.GetSystemDateTime();

            //currentYearSubmission.SetPaymentInformation(request.PaymentReference, request.PaymentReturnToken, request.PaymentId, systemDateTime.Date);

            await weeeContext.SaveChangesAsync();
  
            return true;
        }
    }
}
