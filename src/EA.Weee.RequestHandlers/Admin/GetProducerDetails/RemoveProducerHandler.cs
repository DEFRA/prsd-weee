namespace EA.Weee.RequestHandlers.Admin.GetProducerDetails
{
    using System.Threading.Tasks;
    using DataAccess.Repositories;
    using Prsd.Core.Mediator;
    using Requests.Admin;
    using Security;

    public class RemoveProducerHandler : IRequestHandler<RemoveProducer, int>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IRegisteredProducerRepository registeredProducerRepository;

        public RemoveProducerHandler(IWeeeAuthorization authorization, IRegisteredProducerRepository registeredProducerRepository)
        {
            this.authorization = authorization;
            this.registeredProducerRepository = registeredProducerRepository;
        }

        public async Task<int> HandleAsync(RemoveProducer request)
        {
            authorization.EnsureCanAccessInternalArea();

            var producer = await registeredProducerRepository.Get(request.RegisteredProducerId);

            producer.Unalign();

            return await registeredProducerRepository.SaveChangesAsync();
        }
    }
}
