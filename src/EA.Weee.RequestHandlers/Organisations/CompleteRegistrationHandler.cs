namespace EA.Weee.RequestHandlers.Organisations
{
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain;
    using Domain.Organisation;
    using Domain.Scheme;
    using Prsd.Core.Domain;
    using Prsd.Core.Mediator;
    using Requests.Organisations;

    internal class CompleteRegistrationHandler : IRequestHandler<CompleteRegistration, Guid>
    {
        private readonly WeeeContext db;

        private readonly IUserContext userContext;

        public CompleteRegistrationHandler(WeeeContext context, IUserContext userContext)
        {
            db = context;
            this.userContext = userContext;
        }

        public async Task<Guid> HandleAsync(CompleteRegistration message)
        {
            if (await db.Organisations.FirstOrDefaultAsync(o => o.Id == message.OrganisationId) == null)
            {
                throw new ArgumentException(string.Format("Could not find an organisation with id {0}",
                    message.OrganisationId));
            }

            var organisation = await db.Organisations.SingleAsync(o => o.Id == message.OrganisationId);

            organisation.CompleteRegistration();
            
            //Created PCS created here until AATF/AE are impelemented
            var scheme = new Scheme(message.OrganisationId);
            db.Schemes.Add(scheme);
           
            await db.SaveChangesAsync();
            return organisation.Id;
        }
    }
}
