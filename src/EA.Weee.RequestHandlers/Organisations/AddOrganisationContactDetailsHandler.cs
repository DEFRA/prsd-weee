namespace EA.Weee.RequestHandlers.Organisations
{
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using DataAccess;
    using Mappings;
    using Prsd.Core.Mediator;
    using Requests.Organisations;

    internal class AddOrganisationContactDetailsHandler : IRequestHandler<AddOrganisationContactDetails, Guid>
    {
        private readonly WeeeContext context;

        public AddOrganisationContactDetailsHandler(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<Guid> HandleAsync(AddOrganisationContactDetails message)
        {
            var address = ValueObjectInitializer.CreateAddress(message.OrganisationContactAddress);

            var organisationContactAddress = context.Addresses.Add(address);

            await context.SaveChangesAsync();

            return organisationContactAddress.Id;
        }
    }
}
