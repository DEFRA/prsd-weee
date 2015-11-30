namespace EA.Weee.RequestHandlers.Scheme
{
    using Core.Helpers;
    using DataAccess;
    using Domain.Scheme;
    using EA.Weee.RequestHandlers.Security;
    using Prsd.Core.Mediator;
    using Requests.Scheme;
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;

    public class SetSchemeStatusHandler : IRequestHandler<SetSchemeStatus, Guid>
    {
        private readonly WeeeContext context;
        private readonly IWeeeAuthorization authorization;

        public SetSchemeStatusHandler(WeeeContext context, IWeeeAuthorization authorization)
        {
            this.context = context;
            this.authorization = authorization;
        }

        public async Task<Guid> HandleAsync(SetSchemeStatus message)
        {
            authorization.EnsureCanAccessInternalArea();

            var scheme = await context.Schemes.SingleOrDefaultAsync(s => s.Id == message.PcsId);

            if (scheme != null)
            {
                scheme.SetStatus(message.Status.ToDomainEnumeration<SchemeStatus>());
                await context.SaveChangesAsync();
                return message.PcsId;
            }

            throw new InvalidOperationException(string.Format("Scheme with Id '{0}' does not exist", message.PcsId));
        }
    }
}
