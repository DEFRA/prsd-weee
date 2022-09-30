namespace EA.Weee.RequestHandlers.Mappings
{
    using System.Collections.Generic;
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Domain.AatfReturn;
    using System.Linq;

    public class AatfNameMapping : IMap<List<Aatf>, List<OrganisationSchemeData>>
    {
        private readonly IMapper mapper;

        public AatfNameMapping(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public List<OrganisationSchemeData> Map(List<Aatf> source)
        {
            Condition.Requires(source).IsNotNull();

            return source.Select(e => new OrganisationSchemeData { DisplayName = e.Name, Id = e.Id }).ToList();
        }
    }
}
