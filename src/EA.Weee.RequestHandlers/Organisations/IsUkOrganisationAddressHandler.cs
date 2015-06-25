namespace EA.Weee.RequestHandlers.Organisations
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.DataAccess;
    using EA.Weee.Requests.Organisations;

    internal class IsUkOrganisationAddressHandler : IRequestHandler<IsUkOrganisationAddress, bool>
    {
        private readonly WeeeContext context;

        public IsUkOrganisationAddressHandler(WeeeContext context)
        {
            this.context = context;
        }

        public Task<bool> HandleAsync(IsUkOrganisationAddress message)
        {
            var organisation = context.Organisations.FirstOrDefault(o => o.Id == message.OrganisationId);

            if (organisation == null)
            {
                throw new ArgumentException(string.Format("No organisation found with Id {0}", message.OrganisationId));
            }

            var organisationAddress = organisation.OrganisationAddress;
            return Task.FromResult(organisationAddress.IsUkAddress());
        }
    }
}
