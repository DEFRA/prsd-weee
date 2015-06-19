namespace EA.Weee.RequestHandlers.Organisations
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.DataAccess;
    using EA.Weee.Requests.Organisations;

    internal class CopyOrganisationAddressIntoRegisteredOfficeHandler : IRequestHandler<CopyOrganisationAddressIntoRegisteredOffice, Guid>
    {
        private readonly WeeeContext context;

        public CopyOrganisationAddressIntoRegisteredOfficeHandler(WeeeContext context)
        {
            this.context = context;
        }

        public Task<Guid> HandleAsync(CopyOrganisationAddressIntoRegisteredOffice message)
        {
            var organisation = context.Organisations.FirstOrDefault(o => o.Id == message.OrganisationId);

            if (organisation == null)
            {
                throw new ArgumentException(string.Format("Could not find an organisation with Id {0}", message.OrganisationId));
            }

            organisation.BusinessAddressIsSameAsOrganisationAddress();

            context.SaveChangesAsync();

            return Task.FromResult(organisation.Id);
        }
    }
}
