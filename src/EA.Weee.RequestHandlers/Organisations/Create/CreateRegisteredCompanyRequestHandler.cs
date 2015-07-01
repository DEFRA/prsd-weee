namespace EA.Weee.RequestHandlers.Organisations.Create
{
    using System;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain;
    using Prsd.Core.Mapper;
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
            var organisation = Organisation.CreateRegisteredCompany(message.BusinessName,
                message.CompanyRegistrationNumber, message.TradingName);
            db.Organisations.Add(organisation);
            await db.SaveChangesAsync();

            return organisation.Id;
        }
    }
}
