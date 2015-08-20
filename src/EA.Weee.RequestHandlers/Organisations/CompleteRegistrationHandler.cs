﻿namespace EA.Weee.RequestHandlers.Organisations
{
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.Scheme;
    using Prsd.Core.Domain;
    using Prsd.Core.Mediator;
    using Requests.Organisations;
    using Security;

    internal class CompleteRegistrationHandler : IRequestHandler<CompleteRegistration, Guid>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly WeeeContext context;
        private readonly IUserContext userContext;

        public CompleteRegistrationHandler(IWeeeAuthorization authorization, WeeeContext context, IUserContext userContext)
        {
            this.authorization = authorization;
            this.context = context;
            this.userContext = userContext;
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
