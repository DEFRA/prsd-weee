namespace EA.Weee.RequestHandlers.Admin
{
    using Core.Scheme;
    using Domain.Scheme;
    using EA.Prsd.Core.Domain;
    using EA.Weee.Core.PaymentDetails;
    using EA.Weee.DataAccess;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain.Producer;
    using EA.Weee.Security;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Security;
    using System;
    using System.Threading.Tasks;

    public class AddPaymentDetailsHandler : IRequestHandler<Requests.Admin.AddPaymentDetails, ManualPaymentResult>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess dataAccess;
        private readonly IUserContext userContext;
        private readonly WeeeContext context;

        public AddPaymentDetailsHandler(IWeeeAuthorization authorization, 
            IGenericDataAccess genericDataAccess,
            IUserContext userContext,
            WeeeContext context)
        {
            this.authorization = authorization;
            this.dataAccess = genericDataAccess;
            this.userContext = userContext;
            this.context = context;
        }

        public async Task<ManualPaymentResult> HandleAsync(Requests.Admin.AddPaymentDetails request)
        {
            authorization.EnsureCanAccessInternalArea();
            authorization.EnsureUserInRole(Roles.InternalAdmin);

            var submission = await dataAccess.GetById<DirectProducerSubmission>(request.DirectProducerSubmissionId);

            if (submission.PaymentFinished)
            {
                throw new InvalidOperationException("Manual payment for submission has already been made.");
            }

            submission.ManualPaymentMadeByUserId = userContext.UserId.ToString();
            submission.ManualPaymentDetails = request.PaymentDetailsDescription;
            submission.ManualPaymentMethod = request.PaymentMethod;
            submission.ManualPaymentReceivedDate = request.PaymentRecievedDate.ToDateTime();
            submission.PaymentFinished = true;

            await context.SaveChangesAsync();

            return new ManualPaymentResult
            {
                RegistrationNumber = submission.RegisteredProducer.ProducerRegistrationNumber,
                ComplianceYear = submission.ComplianceYear
            };
        }
    }
}
