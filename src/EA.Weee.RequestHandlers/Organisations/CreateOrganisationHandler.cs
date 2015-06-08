namespace EA.Weee.RequestHandlers.Organisations
{
    using System;
    using System.Data;
    using System.Data.Entity;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using DataAccess;
    using DataAccess.Identity;
    using Domain;
    using Microsoft.AspNet.Identity;
    using Prsd.Core.Domain;
    using Prsd.Core.Mediator;
    using Requests.Organisations;
    using ClaimTypes = Requests.ClaimTypes;
    using Type = Requests.Organisations.Type;

    internal class CreateOrganisationHandler : IRequestHandler<CreateOrganisation, Guid>
    {
        private readonly WeeeContext db;
        private readonly IUserContext userContext;
        private readonly UserManager<ApplicationUser> userManager;

        public CreateOrganisationHandler(WeeeContext db, IUserContext userContext, UserManager<ApplicationUser> userManager)
        {
            this.db = db;
            this.userContext = userContext;
            this.userManager = userManager;
        }

        public async Task<Guid> HandleAsync(CreateOrganisation command)
        {
            OrganisationType type = OrganisationType.RegisteredCompany;
            var orgData = command.Organisation;
            switch (command.Organisation.OrganisationType)
            {
                case Type.Partnership:
                    type = OrganisationType.Partnership;
                    break;

                case Type.RegisteredCompany:
                type = OrganisationType.RegisteredCompany;
                break;

                case Type.SoleTraderOrIndividual:
                type = OrganisationType.SoleTrader;
                break;
            }
            var organisation = new Organisation(command.Organisation.Name, type, OrganisationStatus.Incomplete);

            db.Organisations.Add(organisation);

            await db.SaveChangesAsync();

            await userManager.AddClaimAsync(userContext.UserId.ToString(), new Claim(ClaimTypes.OrganisationId, organisation.Id.ToString()));

            return organisation.Id;
        }
    }
}