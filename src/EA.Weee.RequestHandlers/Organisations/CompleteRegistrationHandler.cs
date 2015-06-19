namespace EA.Weee.RequestHandlers.Organisations
{
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using DataAccess;
    using Prsd.Core.Mediator;
    using Requests.Organisations;

    internal class CompleteRegistrationHandler : IRequestHandler<CompleteRegistration, Guid>
    {
        private readonly WeeeContext db;

        public CompleteRegistrationHandler(WeeeContext context)
        {
            db = context;
        }

        public async Task<Guid> HandleAsync(CompleteRegistration message)
        {
            var organisation = await db.Organisations.SingleAsync(o => o.Id == message.OrganisationId);
            organisation.ToPending();
            await db.SaveChangesAsync();
            return organisation.Id;
        }
    }
}
