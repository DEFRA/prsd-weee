namespace EA.Weee.RequestHandlers.Scheme
{
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using Core.Helpers;
    using DataAccess;
    using Domain.Scheme;
    using Prsd.Core.Mediator;
    using Requests.Scheme;

    public class SetSchemeStatusHandler : IRequestHandler<SetSchemeStatus, Guid>
    {
        private readonly WeeeContext context;

        public SetSchemeStatusHandler(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<Guid> HandleAsync(SetSchemeStatus message)
        {
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
