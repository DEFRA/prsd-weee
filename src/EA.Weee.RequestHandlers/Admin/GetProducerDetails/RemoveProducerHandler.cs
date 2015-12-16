namespace EA.Weee.RequestHandlers.Admin.GetProducerDetails
{
    using System;
    using System.Threading.Tasks;
    using Core.Admin;
    using DataAccess.Repositories;
    using Prsd.Core.Mediator;
    using Requests.Admin;
    using Security;

    public class RemoveProducerHandler : IRequestHandler<RemoveProducer, RemoveProducerResult>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IRegisteredProducerRepository registeredProducerRepository;

        public RemoveProducerHandler(IWeeeAuthorization authorization, IRegisteredProducerRepository registeredProducerRepository)
        {
            this.authorization = authorization;
            this.registeredProducerRepository = registeredProducerRepository;
        }

        public async Task<RemoveProducerResult> HandleAsync(RemoveProducer request)
        {
            authorization.EnsureCanAccessInternalArea();

            var producer = await registeredProducerRepository.GetProducerRegistration(request.RegisteredProducerId);

            producer.Unalign();

            await registeredProducerRepository.SaveChangesAsync();

            return new RemoveProducerResult(true);
        }
    }
}
