namespace EA.Weee.RequestHandlers.Admin.GetProducerDetails
{
    using System.Threading.Tasks;
    using Prsd.Core.Mediator;
    using Security;

    public class RemoveProducerHandler : IRequestHandler<Requests.Admin.RemoveProducer, bool>
    {
        private readonly IWeeeAuthorization authorization;

        public RemoveProducerHandler(IWeeeAuthorization authorization)
        {
            this.authorization = authorization;
        }

        public async Task<bool> HandleAsync(Requests.Admin.RemoveProducer request)
        {
            authorization.EnsureCanAccessInternalArea();

            // TODO: The actual implementation
            return await Task.Run(() => true);
        }
    }
}
