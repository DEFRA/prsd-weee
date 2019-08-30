namespace EA.Prsd.Core.Decorators
{
    using System.Reflection;
    using System.Security.Authentication;
    using System.Threading.Tasks;
    using Domain;
    using Mediator;
    using Security;

    public class AuthenticationRequestHandlerDecorator<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IRequestHandler<TRequest, TResponse> inner;
        private readonly IUserContext userContext;

        public AuthenticationRequestHandlerDecorator(IRequestHandler<TRequest, TResponse> inner,
            IUserContext userContext)
        {
            this.inner = inner;
            this.userContext = userContext;
        }

        public async Task<TResponse> HandleAsync(TRequest message)
        {
            var unauthorizedAttribute = typeof(TRequest).GetCustomAttribute<AllowUnauthorizedUserAttribute>();
            var hasAccess = unauthorizedAttribute != null || userContext.Principal.Identity.IsAuthenticated;

            if (hasAccess)
            {
                return await inner.HandleAsync(message);
            }

            // TODO - need more detail in the exception
            throw new AuthenticationException("Access denied.");
        }
    }
}