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

    public class ReturnMap : IMap<ReturnQuarterWindow, ReturnData>
    {
        public ReturnData Map(ReturnQuarterWindow source)
        {
            Guard.ArgumentNotNull(() => source, source);

            return new ReturnData()
            {
                Id = source.Return.Id,
                Quarter = new Quarter(source.Return.Quarter.Year, (QuarterType)source.Return.Quarter.Q),
                QuarterWindow = new QuarterWindow(source.QuarterWindow.StartDate, source.QuarterWindow.EndDate)
            };
        }
    }
}
