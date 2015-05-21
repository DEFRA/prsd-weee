namespace EA.Prsd.Core.Decorators
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Security;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Mediator;
    using Security;

    public class AuthorizationCommandHandlerDecorator<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IRequestHandler<TRequest, TResponse> inner;
        private readonly IResourceAuthorizationManager manager;
        private readonly ClaimsPrincipal principal;

        public AuthorizationCommandHandlerDecorator(IRequestHandler<TRequest, TResponse> inner,
            IResourceAuthorizationManager manager, ClaimsPrincipal principal)
        {
            this.inner = inner;
            this.manager = manager;
            this.principal = principal;
        }

        public async Task<TResponse> HandleAsync(TRequest message)
        {
            var permissionAttribute = typeof(TRequest).GetCustomAttribute<ResourceAuthorizeAttribute>();
            bool hasAccess;

            if (permissionAttribute == null)
            {
                // TODO - throw exception or allow commands with no permissions?
                hasAccess = true;
            }
            else
            {
                var context = new ResourceAuthorizationContext(principal,
                    new[] { ActionFromAttribute(permissionAttribute) }, ResourcesFromAttribute(permissionAttribute));
                hasAccess = await manager.CheckAccessAsync(context);
            }

            if (hasAccess)
            {
                return await inner.HandleAsync(message);
            }

            throw new SecurityException("Access denied.");
        }

        private Claim ActionFromAttribute(ResourceAuthorizeAttribute attribute)
        {
            return !string.IsNullOrWhiteSpace(attribute.Action) ? new Claim("name", attribute.Action) : null;
        }

        private List<Claim> ResourcesFromAttribute(ResourceAuthorizeAttribute attribute)
        {
            if ((attribute.Resources != null) && (attribute.Resources.Any()))
            {
                return attribute.Resources.Select(r => new Claim("name", r)).ToList();
            }

            return null;
        }
    }
}