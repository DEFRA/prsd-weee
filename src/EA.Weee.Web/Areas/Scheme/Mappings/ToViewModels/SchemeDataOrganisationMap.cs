namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using System.Collections.Generic;
    using Core.Scheme;
    using CuttingEdge.Conditions;
    using EA.Weee.Core.Shared;
    using Prsd.Core.Mapper;

    public class SchemeDataOrganisationMap : IMap<List<SchemeData>, List<EntityIdDisplayNameData>>
    {
        private readonly IMapper mapper;

        public SchemeDataOrganisationMap(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public List<EntityIdDisplayNameData> Map(List<SchemeData> source)
        {
            Condition.Requires(source).IsNotNull().IsNotEmpty();

            var result = new List<EntityIdDisplayNameData>();

            foreach (var data in source)
            {
                result.Add(new EntityIdDisplayNameData
                {
                     DisplayName = data.SchemeName,
                     Id = data.OrganisationId
                });
            }

            return result;
        }
    }
}
