namespace EA.Weee.RequestHandlers.Admin.GetSchemes
{
    using Core.Scheme;
    using Domain.Scheme;
    using EA.Weee.Core.PaymentDetails;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain.Producer;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Security;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class GetPaymentHandler : IRequestHandler<Requests.Admin.GetPaymentDetails, OfflinePaymentDetails>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess dataAccess;
        private readonly IMap<Scheme, SchemeData> schemeMap;

        public GetPaymentHandler(IWeeeAuthorization authorization, IMap<Scheme, SchemeData> schemeMap, IGenericDataAccess genericDataAccess)
        {
            this.authorization = authorization;
            this.dataAccess = genericDataAccess;
            this.schemeMap = schemeMap;
        }

        public async Task<OfflinePaymentDetails> HandleAsync(Requests.Admin.GetPaymentDetails request)
        {
            authorization.EnsureCanAccessInternalArea();

            var submission = await dataAccess.GetById<DirectProducerSubmission>(request.DirectProducerSubmissionId);

            return new OfflinePaymentDetails
            {
                PaymentDetailsDescription = submission.ManualPaymentDetails,
                PaymentMethod = submission.ManualPaymentMethod,
                PaymentRecievedDate = submission.ManualPaymentReceivedDate
            };
        }
    }
}
