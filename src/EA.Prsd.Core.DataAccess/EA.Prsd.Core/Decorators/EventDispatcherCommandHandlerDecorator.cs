namespace EA.Prsd.Core.Decorators
{
    using System.Threading.Tasks;
    using Domain;
    using Mediator;

    public class EventDispatcherCommandHandlerDecorator<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IDeferredEventDispatcher eventDispatcher;
        private readonly IRequestHandler<TRequest, TResponse> inner;

        public EventDispatcherCommandHandlerDecorator(IRequestHandler<TRequest, TResponse> inner,
            IDeferredEventDispatcher eventDispatcher)
        {
            this.inner = inner;
            this.eventDispatcher = eventDispatcher;
        }

        public async Task<TResponse> HandleAsync(TRequest message)
        {
            DomainEvents.Dispatcher = eventDispatcher;
            var result = await inner.HandleAsync(message);
            await eventDispatcher.Resolve();
            return result;
        }
    }
}