namespace EA.Weee.RequestHandlers.Mappings
{
    using Core.Scheme;
    using CuttingEdge.Conditions;
    using Domain.Scheme;
    using Prsd.Core.Mapper;

    public class SchemeOrganisationMap : IMap<Scheme, OrganisationSchemeData>
    {
        private readonly IMapper mapper;

        public SchemeOrganisationMap(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public OrganisationSchemeData Map(Scheme source)
        {
            Condition.Requires(source).IsNotNull();

            return new OrganisationSchemeData
            {
                DisplayName = source.SchemeName,
                OrganisationId = source.OrganisationId
            };
        }
    }
}
