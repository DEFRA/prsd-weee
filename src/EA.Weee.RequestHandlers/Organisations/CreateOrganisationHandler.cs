namespace EA.Weee.RequestHandlers.Organisations
{
    using System;
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
    using OrganisationType = Domain.OrganisationType;

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
            OrganisationType organisationType;
      
            switch (command.Organisation.OrganisationType)
            {
                case Requests.Organisations.OrganisationType.Partnership:
                    organisationType = OrganisationType.Partnership;
                    break;

                case Requests.Organisations.OrganisationType.RegisteredCompany:
                organisationType = OrganisationType.RegisteredCompany;
                break;

                case Requests.Organisations.OrganisationType.SoleTraderOrIndividual:
                organisationType = OrganisationType.SoleTraderOrIndividual;
                break;

                default:
                throw new InvalidOperationException(string.Format("Unknown organisation type: {0}", command.Organisation.OrganisationType));
            }

            var organisation = new Organisation(command.Organisation.Name, organisationType, OrganisationStatus.Incomplete);

            db.Organisations.Add(organisation);

            await db.SaveChangesAsync();

            await userManager.AddClaimAsync(userContext.UserId.ToString(), new Claim(ClaimTypes.OrganisationId, organisation.Id.ToString()));

            return organisation.Id;
        }
    }
}