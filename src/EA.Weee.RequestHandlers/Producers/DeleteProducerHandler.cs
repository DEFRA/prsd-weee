namespace EA.Weee.RequestHandlers.Producers
{
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Prsd.Core.Mediator;
    using Requests.Producers;

    internal class DeleteProducerHandler : IRequestHandler<DeleteProducer, bool>
    {
        private readonly IwsContext context;

        public DeleteProducerHandler(IwsContext context)
        {
            this.context = context;
        }

        public async Task<bool> HandleAsync(DeleteProducer query)
        {
            var producer = await context.Producers.SingleAsync(p => p.Id == query.ProducerId);
            var notification = await context.NotificationApplications.Where(n => n.Id == query.NotificationId).SingleAsync();
            notification.RemoveProducer(producer);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
