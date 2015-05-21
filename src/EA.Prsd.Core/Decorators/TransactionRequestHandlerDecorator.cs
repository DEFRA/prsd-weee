namespace EA.Prsd.Core.Decorators
{
    using System.Threading.Tasks;
    using System.Transactions;
    using Mediator;

    public class TransactionRequestHandlerDecorator<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IRequestHandler<TRequest, TResponse> inner;

        public TransactionRequestHandlerDecorator(IRequestHandler<TRequest, TResponse> inner)
        {
            this.inner = inner;
        }

        public async Task<TResponse> HandleAsync(TRequest message)
        {
            using (var scope = new TransactionScope())
            {
                var result = await inner.HandleAsync(message).ConfigureAwait(true);

                scope.Complete();

                return result;
            }
        }
    }
}