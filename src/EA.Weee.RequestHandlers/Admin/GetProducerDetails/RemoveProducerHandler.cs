namespace EA.Weee.RequestHandlers.Admin.GetProducerDetails
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Admin;
    using DataAccess.DataAccess;
    using Prsd.Core.Mediator;
    using Requests.Admin;
    using Security;

    public class RemoveProducerHandler : IRequestHandler<RemoveProducer, RemoveProducerResult>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IRemoveProducerDataAccess removeProducerDataAccess;

        public RemoveProducerHandler(IWeeeAuthorization authorization, IRemoveProducerDataAccess removeProducerDataAccess)
        {
            this.authorization = authorization;
            this.removeProducerDataAccess = removeProducerDataAccess;
        }

        public async Task<RemoveProducerResult> HandleAsync(RemoveProducer request)
        {
            authorization.EnsureCanAccessInternalArea();

            var producerSubmissions =
                await removeProducerDataAccess.GetProducerSubmissionsForRegisteredProducer(request.RegisteredProducerId);

            foreach (var producerSubmission in producerSubmissions)
            {
                if (producerSubmission.MemberUpload.InvoiceRun == null)
                {
                    producerSubmission.MemberUpload.DeductFromTotalCharges(producerSubmission.ChargeThisUpdate);
                }
            }

            var producer = await removeProducerDataAccess.GetProducerRegistration(request.RegisteredProducerId);

            producer.Unalign();

            await removeProducerDataAccess.SaveChangesAsync();

            return new RemoveProducerResult(true);
        }
    }
}
