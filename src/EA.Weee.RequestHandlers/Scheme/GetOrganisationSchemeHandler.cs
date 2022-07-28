namespace EA.Weee.RequestHandlers.Scheme
{
    using Core.Scheme;
    using Domain.Scheme;
    using EA.Weee.RequestHandlers.Security;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Requests.Scheme;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using EA.Weee.Domain.Organisation;

    public class GetOrganisationSchemeHandler : IRequestHandler<GetOrganisationScheme, List<OrganisationSchemeData>>
    {
        private readonly IGetSchemesDataAccess dataAccess;
        private readonly IMap<Scheme, OrganisationSchemeData> schemeOrganisationMap;
        private readonly IMap<ProducerBalancingScheme, OrganisationSchemeData> producerBalancingSchemeOrganisationMap;
        private readonly IWeeeAuthorization authorization;

        public GetOrganisationSchemeHandler(
         IGetSchemesDataAccess dataAccess,
         IMap<Scheme, OrganisationSchemeData> schemeOrganisationMap,
         IMap<ProducerBalancingScheme, OrganisationSchemeData> producerBalancingSchemeOrganisationMap,
         IWeeeAuthorization authorization)
        {
            this.dataAccess = dataAccess;
            this.schemeOrganisationMap = schemeOrganisationMap;
            this.producerBalancingSchemeOrganisationMap = producerBalancingSchemeOrganisationMap;
            this.authorization = authorization;
        }
        public async Task<List<OrganisationSchemeData>> HandleAsync(GetOrganisationScheme message)
        {
            authorization.EnsureCanAccessExternalArea();

            var schemes = await dataAccess.GetCompleteSchemes();

            var pbs = await dataAccess.GetProducerBalancingScheme();

            var mappedSchemes = schemes.Where(s => s.SchemeStatus == SchemeStatus.Approved)
                .Select(s => schemeOrganisationMap.Map(s))
                .ToList();

            var mappedPbs = producerBalancingSchemeOrganisationMap.Map(pbs);

            if (message.IncludePBS)
            {
                mappedSchemes.Add(mappedPbs);
            }

            return mappedSchemes.OrderBy(m => m.DisplayName).ToList();
        }
    }
}
