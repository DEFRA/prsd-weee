namespace EA.Weee.RequestHandlers.Mappings
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Domain.DataReturns;
    using EA.Weee.Domain.Obligation;
    using System.Collections.Generic;
    using System.Linq;

    internal class TonnageDataMap : IMap<EeeOutputReturnVersion, IList<TonnageData>>
    {
        public IList<TonnageData> Map(EeeOutputReturnVersion source)
        {
            if (source == null)
            {
                return new List<TonnageData>();
            }

            return source.EeeOutputAmounts
                .GroupBy(ri => ri.WeeeCategory)
                .Select(group =>
                {
                    decimal? houseHold = null;
                    decimal? nonHouseHold = null;

                    var houseHoldItems = group.Where(ri => ri.ObligationType == ObligationType.B2C).ToList();
                    var nonHouseHoldItems = group.Where(ri => ri.ObligationType == ObligationType.B2B).ToList();

                    if (houseHoldItems.Any())
                    {
                        houseHold = houseHoldItems.Sum(ri => ri.Tonnage);
                    }

                    if (nonHouseHoldItems.Any())
                    {
                        nonHouseHold = nonHouseHoldItems.Sum(ri => ri.Tonnage);
                    }

                    return new TonnageData(houseHold, nonHouseHold, group.Key);
                }).ToList();
        }
    }
}
