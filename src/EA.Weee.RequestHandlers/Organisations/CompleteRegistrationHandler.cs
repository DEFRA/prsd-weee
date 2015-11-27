namespace EA.Weee.RequestHandlers.Organisations
{
    using DataAccess;
    using Domain.Scheme;
    using Prsd.Core.Mediator;
    using Requests.Organisations;
    using Security;
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;

    internal class CompleteRegistrationHandler : IRequestHandler<CompleteRegistration, Guid>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly WeeeContext context;
  
        public CompleteRegistrationHandler(IWeeeAuthorization authorization, WeeeContext context)
        {
            this.authorization = authorization;
            this.context = context;
        }

        public async Task<Guid> HandleAsync(CompleteRegistration message)
        {
            authorization.EnsureOrganisationAccess(message.OrganisationId);

            if (await context.Organisations.FirstOrDefaultAsync(o => o.Id == message.OrganisationId) == null)
            {
                throw new ArgumentException(string.Format("Could not find an organisation with id {0}",
                    message.OrganisationId));
            }

            var organisation = await context.Organisations.SingleAsync(o => o.Id == message.OrganisationId);

            organisation.CompleteRegistration();
            
            //Created PCS created here until AATF/AE are impelemented
            var scheme = new Scheme(message.OrganisationId);
            context.Schemes.Add(scheme);
           
            await context.SaveChangesAsync();
            return organisation.Id;
        }
    }
}
