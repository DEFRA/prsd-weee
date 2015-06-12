namespace EA.Weee.RequestHandlers.Organisations.Create
{
    using System;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain;
    using Prsd.Core.Mediator;
    using Requests.Organisations.Create;

    public class CreateRegisteredCompanyRequestHandler : IRequestHandler<CreateRegisteredCompanyRequest, Guid>
    {
        private readonly WeeeContext db;

        public CreateRegisteredCompanyRequestHandler(WeeeContext db)
        {
            this.db = db;
        }

        public async Task<Guid> HandleAsync(CreateRegisteredCompanyRequest message)
        {
            var organisation = new Organisation(message.BusinessName, null, OrganisationType.RegisteredCompany, OrganisationStatus.Incomplete)
            {
                CompanyRegistrationNumber = message.CompanyRegistrationNumber
            };

            db.Organisations.Add(organisation);
            await db.SaveChangesAsync();

            return organisation.Id;
        }
    }
}
