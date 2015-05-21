namespace EA.Prsd.Core.Autofac
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using global::Autofac;
    using Mediator;

    public class AutofacMediator : IMediator
    {
        private readonly IComponentContext context;

        public AutofacMediator(IComponentContext context)
        {
            this.context = context;
        }

        public async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
        {
            return await SendAsync<TResponse>(request, request.GetType(), typeof(TResponse));
        }

        public async Task<object> SendAsync(object request, Type responseType)
        {
            return await SendAsync<object>(request, request.GetType(), responseType);
        }

        [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1008:OpeningParenthesisMustBeSpacedCorrectly",
            Justification = "Spacing around dynamic cast for readability.")]
        private async Task<TResponse> SendAsync<TResponse>(object request, Type requestType, Type responseType)
        {
            var handler = GetHandler(requestType, responseType);

            var requestAsyncMethod = handler.GetType().GetMethod("HandleAsync");

            // This dynamically awaited task will return the correct result, allowing us to await a task we do not know the type of.
            return await (dynamic)requestAsyncMethod.Invoke(handler, new[] { request });
        }

        private object GetHandler(Type requestType, Type responseType)
        {
            var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);

            object handler;

            try
            {
                handler = context.Resolve(handlerType);
            }
            catch (Exception ex)
            {
                throw new MissingHandlerException(string.Format("No handler found for request {0} with response {1}",
                    requestType.FullName,
                    responseType.FullName), ex);
            }

            return handler;
        }
    }
}