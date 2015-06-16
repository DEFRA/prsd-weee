namespace EA.Weee.RequestHandlers.Organisations
{
    using System;
    using System.Threading.Tasks;
    using Prsd.Core.Mediator;
    using Requests.Organisations;

    public class GetOrganisationPrincipalPlaceOfBusinessHandler : IRequestHandler<GetOrganisationPrincipalPlaceOfBusiness, OrganisationData> 
    {
        public async Task<OrganisationData> HandleAsync(GetOrganisationPrincipalPlaceOfBusiness message)
        {
            // TODO: Implement
            Func<OrganisationData> orgData = () => new OrganisationData
            {
                Id = message.OrganisationId
            };

            return await Task.Run(orgData);
        }
    }
}
