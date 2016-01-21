namespace EA.Weee.RequestHandlers.Mappings
{
    using System;
    using Domain.Lookup;
    using Prsd.Core.Mapper;

    public class WeeeCategoryMapping : IMap<WeeeCategory, Core.DataReturns.WeeeCategory>
    {
        public Core.DataReturns.WeeeCategory Map(WeeeCategory source)
        {
            return (Core.DataReturns.WeeeCategory)(int)source;
        }
    }
}
