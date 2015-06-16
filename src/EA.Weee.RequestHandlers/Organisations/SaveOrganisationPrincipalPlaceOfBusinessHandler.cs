namespace EA.Weee.RequestHandlers.Organisations
{
    using System;
    using System.Threading.Tasks;
    using Prsd.Core.Mediator;
    using Requests.Organisations;

    public class SaveOrganisationPrincipalPlaceOfBusinessHandler : IRequestHandler<SaveOrganisationPrincipalPlaceOfBusiness, Guid>
    {
        public async Task<Guid> HandleAsync(SaveOrganisationPrincipalPlaceOfBusiness message)
        {
            // TODO: Implement
            Func<Guid> orgData = Guid.NewGuid;
            return await Task.Run(orgData);
        }
    }
}
