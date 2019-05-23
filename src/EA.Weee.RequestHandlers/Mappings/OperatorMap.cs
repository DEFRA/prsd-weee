namespace EA.Weee.RequestHandlers.Mappings
{
    using Core.AatfReturn;
    using Domain.AatfReturn;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Domain.Organisation;
    using Prsd.Core;
    using Prsd.Core.Mapper;

    public class OperatorMap : IMap<Operator, OperatorData>
    {
        private readonly IMap<Organisation, OrganisationData> organisationMapper;

        public OperatorMap(IMap<Organisation, OrganisationData> organisationMapper)
        {
            this.organisationMapper = organisationMapper;
        }

        public OperatorData Map(Operator source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var organisation = organisationMapper.Map(source.Organisation);

            return new OperatorData(source.Id, source.Organisation.Name, organisation, source.Organisation.Id);
        }
    }
}
