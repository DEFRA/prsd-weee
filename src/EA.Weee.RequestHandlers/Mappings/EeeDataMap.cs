namespace EA.Weee.RequestHandlers.Mappings
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Domain.DataReturns;
    using EA.Weee.Domain.Obligation;
    using System.Collections.Generic;
    using System.Linq;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Core.Helpers;

    internal class EeeDataMap : IMap<EeeOutputReturnVersion, IList<Eee>>
    {
        public IList<Eee> Map(EeeOutputReturnVersion source)
        {
            if (source == null)
            {
                return new List<Eee>();
            }

            return source.EeeOutputAmounts.Select(item => new Eee(item.Tonnage,
                (WeeeCategory)item.WeeeCategory,
                (Core.Shared.ObligationType)item.ObligationType)).ToList();
        }
    }
}
