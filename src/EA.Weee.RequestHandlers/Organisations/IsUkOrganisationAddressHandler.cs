namespace EA.Weee.RequestHandlers.Organisations
{
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using DataAccess;
    using Prsd.Core.Mediator;
    using Requests.Organisations;

    internal class IsUkOrganisationAddressHandler : IRequestHandler<IsUkOrganisationAddress, bool>
    {
        private readonly WeeeContext context;

        public IsUkOrganisationAddressHandler(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<bool> HandleAsync(IsUkOrganisationAddress message)
        {
            var organisation = await context.Organisations.FirstOrDefaultAsync(o => o.Id == message.OrganisationId);

            if (organisation == null)
            {
                throw new ArgumentException(string.Format("No organisation found with Id {0}", message.OrganisationId));
            }

            return organisation.OrganisationAddress.IsUkAddress();
        }
    }
}
