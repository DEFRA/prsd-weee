namespace EA.Weee.RequestHandlers.Mappings
{
    using System.Collections.Generic;
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Domain.AatfReturn;
    using System.Linq;
    using EA.Weee.Core.Shared;

    public class AatfNameMapping : IMap<List<Aatf>, List<EntityIdDisplayNameData>>
    {
        private readonly IMapper mapper;

        public AatfNameMapping(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public List<EntityIdDisplayNameData> Map(List<Aatf> source)
        {
            Condition.Requires(source).IsNotNull();

            return source.Select(e => new EntityIdDisplayNameData { DisplayName = e.Name, Id = e.Id }).ToList();
        }
    }
}
