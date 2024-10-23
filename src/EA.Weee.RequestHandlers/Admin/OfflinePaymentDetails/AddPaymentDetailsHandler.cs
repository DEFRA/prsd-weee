namespace EA.Weee.RequestHandlers.Admin.GetSchemes
{
    using Core.Scheme;
    using Domain.Scheme;
    using EA.Prsd.Core.Domain;
    using EA.Weee.Core.PaymentDetails;
    using EA.Weee.DataAccess;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain.Producer;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Security;
    using System;
    using System.Threading.Tasks;

    public class AddPaymentDetailsHandler : IRequestHandler<Requests.Admin.AddPaymentDetails, OfflinePaymentDetails>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess dataAccess;
        private readonly IUserContext userContext;
        private readonly WeeeContext context;
        private readonly IMap<Scheme, SchemeData> schemeMap;

        public AddPaymentDetailsHandler(IWeeeAuthorization authorization, 
            IMap<Scheme, SchemeData> schemeMap, 
            IGenericDataAccess genericDataAccess,
            IUserContext userContext,
            WeeeContext context)
        {
            this.authorization = authorization;
            this.dataAccess = genericDataAccess;
            this.userContext = userContext;
            this.context = context;
            this.schemeMap = schemeMap;
        }

        public async Task<OfflinePaymentDetails> HandleAsync(Requests.Admin.AddPaymentDetails request)
        {
            authorization.EnsureCanAccessInternalArea();

            var submission = await dataAccess.GetById<DirectProducerSubmission>(request.DirectProducerSubmissionId);

            submission.ManualPaymentMadeByUserId = userContext.UserId.ToString();
            submission.ManualPaymentDetails = request.PaymentDetailsDescription;
            submission.ManualPaymentMethod = request.PaymentMethod;
            submission.ManualPaymentReceivedDate = request.PaymentRecievedDate;
            submission.PaymentFinished = true;
            submission.FinalPaymentSession.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return new OfflinePaymentDetails
            {
                RegistrationNumber = submission.RegisteredProducer.ProducerRegistrationNumber,
                PaymentDetailsDescription = submission.ManualPaymentDetails,
                PaymentMethod = submission.ManualPaymentMethod,
                PaymentRecievedDate = submission.ManualPaymentReceivedDate
            };
        }
    }
}
