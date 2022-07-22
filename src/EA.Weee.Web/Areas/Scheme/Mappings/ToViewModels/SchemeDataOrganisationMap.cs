namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using System.Collections.Generic;
    using Core.Scheme;
    using CuttingEdge.Conditions;
    using Prsd.Core.Mapper;

    public class SchemeDataOrganisationMap : IMap<List<SchemeData>, List<OrganisationSchemeData>>
    {
        private readonly IMapper mapper;

        public SchemeDataOrganisationMap(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public List<OrganisationSchemeData> Map(List<SchemeData> source)
        {
            Condition.Requires(source).IsNotNull().IsNotEmpty();

            var result = new List<OrganisationSchemeData>();

            foreach (var data in source)
            {
                result.Add(new OrganisationSchemeData
                {
                     DisplayName = data.SchemeName,
                     Id = data.OrganisationId
                });
            }

            return result;
        }
    }
}
