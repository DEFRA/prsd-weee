namespace EA.Weee.RequestHandlers.Mappings
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.Shared;
    using EA.Weee.Domain.Organisation;

    public class OrganisationProducerBalancingSchemeMap : IMap<ProducerBalancingScheme, EntityIdDisplayNameData>
    {
        private readonly IMapper mapper;

        public OrganisationProducerBalancingSchemeMap(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public EntityIdDisplayNameData Map(ProducerBalancingScheme source)
        {
            return new EntityIdDisplayNameData
            {
                DisplayName = source.Organisation.Name,
                Id = source.Organisation.Id
            };
        }
    }
}
