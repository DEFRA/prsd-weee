namespace EA.Weee.RequestHandlers.Mappings
{
    using Core.Scheme;
    using Domain.AatfReturn;
    using Domain.Scheme;
    using Prsd.Core.Mapper;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Core.AatfReturn;
    using Core.DataReturns;
    using Prsd.Core;

    public class ReturnMap : IMap<Return, ReturnData>
    {
        public ReturnData Map(Return source)
        {
            Guard.ArgumentNotNull(() => source, source);

            return new ReturnData()
            {
                Id = source.Id,
                Quarter = new Quarter(source.Quarter.Year, (QuarterType)source.Quarter.Q)
            };
        }
    }
}
