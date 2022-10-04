namespace EA.Weee.RequestHandlers.Scheme
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.Scheme;
    using EA.Weee.Core.Shared;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.RequestHandlers.Security;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Requests.Scheme;

    public class GetOrganisationSchemeHandler : IRequestHandler<GetOrganisationScheme, List<EntityIdDisplayNameData>>
    {
        private readonly IGetSchemesDataAccess dataAccess;
        private readonly IMap<Scheme, EntityIdDisplayNameData> schemeOrganisationMap;
        private readonly IMap<ProducerBalancingScheme, EntityIdDisplayNameData> producerBalancingSchemeOrganisationMap;
        private readonly IWeeeAuthorization authorization;

        public GetOrganisationSchemeHandler(
         IGetSchemesDataAccess dataAccess,
         IMap<Scheme, EntityIdDisplayNameData> schemeOrganisationMap,
         IMap<ProducerBalancingScheme, EntityIdDisplayNameData> producerBalancingSchemeOrganisationMap,
         IWeeeAuthorization authorization)
        {
            this.dataAccess = dataAccess;
            this.schemeOrganisationMap = schemeOrganisationMap;
            this.producerBalancingSchemeOrganisationMap = producerBalancingSchemeOrganisationMap;
            this.authorization = authorization;
        }
        public async Task<List<EntityIdDisplayNameData>> HandleAsync(GetOrganisationScheme message)
        {
            authorization.EnsureCanAccessExternalArea();

            var schemes = await dataAccess.GetCompleteSchemes();

            var pbs = await dataAccess.GetProducerBalancingScheme();

            var mappedSchemes = schemes.Where(s => s.SchemeStatus == Domain.Scheme.SchemeStatus.Approved)
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
