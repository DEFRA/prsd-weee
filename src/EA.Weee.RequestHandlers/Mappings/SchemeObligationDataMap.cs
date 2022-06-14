namespace EA.Weee.RequestHandlers.Mappings
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.Admin.Obligation;
    using CuttingEdge.Conditions;
    using Domain.Scheme;
    using EA.Weee.Core.DataReturns;
    using Prsd.Core.Mapper;

    public class SchemeObligationDataMap : IMap<List<Scheme>, List<SchemeObligationData>>
    {
        public List<SchemeObligationData> Map(List<Scheme> source)
        {
            Condition.Requires(source).IsNotNull();

            var returnData = new List<SchemeObligationData>();

            foreach (var scheme in source)
            {
                if (scheme.ObligationSchemes.Any())
                {
                    returnData.Add(new SchemeObligationData(scheme.SchemeName,
                        scheme.ObligationSchemes.First().UpdatedDate,
                        scheme.ObligationSchemes.First().ObligationSchemeAmounts.Select(os => new SchemeObligationAmountData((WeeeCategory)os.CategoryId, os.Obligation)).ToList()));
                }
                else
                {
                    returnData.Add(new SchemeObligationData(scheme.SchemeName, null,
                        new List<SchemeObligationAmountData>()));
                }
            }

            return returnData;
        }
    }
}
