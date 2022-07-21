namespace EA.Weee.RequestHandlers.Mappings
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Domain.Organisation;

    public class OrganisationProducerBalancingSchemeMap : IMap<ProducerBalancingScheme, OrganisationSchemeData>
    {
        private readonly IMapper mapper;

        public OrganisationProducerBalancingSchemeMap(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public OrganisationSchemeData Map(ProducerBalancingScheme source)
        {
            return new OrganisationSchemeData
            {
                DisplayName = source.Organisation.Name,
                Id = source.Organisation.Id
            };
        }
    }
}
