namespace EA.Prsd.Core.Decorators
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Security;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Domain;
    using Mediator;
    using Security;

    public class AuthorizationRequestHandlerDecorator<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IRequestHandler<TRequest, TResponse> inner;
        private readonly IResourceAuthorizationManager manager;
        private readonly IUserContext userContext;

        public AuthorizationRequestHandlerDecorator(IRequestHandler<TRequest, TResponse> inner,
            IResourceAuthorizationManager manager, IUserContext userContext)
        {
            this.inner = inner;
            this.manager = manager;
            this.userContext = userContext;
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
                var context = new ResourceAuthorizationContext(userContext.Principal,
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