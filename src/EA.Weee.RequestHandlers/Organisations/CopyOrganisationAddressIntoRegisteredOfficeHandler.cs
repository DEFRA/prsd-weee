namespace EA.Weee.RequestHandlers.Organisations
{
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain;
    using Domain.Organisation;
    using Prsd.Core.Mediator;
    using Requests.Organisations;
    using Security;

    internal class CopyOrganisationAddressIntoRegisteredOfficeHandler :
        IRequestHandler<CopyOrganisationAddressIntoRegisteredOffice, Guid>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly WeeeContext context;

        public CopyOrganisationAddressIntoRegisteredOfficeHandler(IWeeeAuthorization authorization, WeeeContext context)
        {
            this.authorization = authorization;
            this.context = context;
        }

        public async Task<Guid> HandleAsync(CopyOrganisationAddressIntoRegisteredOffice message)
        {
            authorization.EnsureOrganisationAccess(message.OrganisationId);

            var organisation = await context.Organisations.FirstOrDefaultAsync(o => o.Id == message.OrganisationId);

            if (organisation == null)
            {
                throw new ArgumentException(string.Format("Could not find an organisation with Id {0}",
                    message.OrganisationId));
            }

            var oa = organisation.OrganisationAddress;

            // we're explicitly making a copy here rather than pointing at the same address row
            // this is only assumed to be the preferred option
            var businessAddress = new Address(
                oa.Address1,
                oa.Address2,
                oa.TownOrCity,
                oa.CountyOrRegion,
                oa.Postcode,
                oa.Country,
                oa.Telephone,
                oa.Email);

            organisation.AddOrUpdateAddress(AddressType.RegisteredOrPPBAddress, businessAddress);

            await context.SaveChangesAsync();

            return organisation.Id;
        }
    }
}
