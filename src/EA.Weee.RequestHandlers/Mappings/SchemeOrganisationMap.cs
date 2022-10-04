namespace EA.Weee.RequestHandlers.Mappings
{
    using CuttingEdge.Conditions;
    using Domain.Scheme;
    using EA.Weee.Core.Shared;
    using Prsd.Core.Mapper;

    public class SchemeOrganisationMap : IMap<Scheme, EntityIdDisplayNameData>
    {
        private readonly IMapper mapper;

        public SchemeOrganisationMap(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public EntityIdDisplayNameData Map(Scheme source)
        {
            Condition.Requires(source).IsNotNull();

            return new EntityIdDisplayNameData
            {
                DisplayName = source.SchemeName,
                Id = source.OrganisationId
            };
        }
    }
}
