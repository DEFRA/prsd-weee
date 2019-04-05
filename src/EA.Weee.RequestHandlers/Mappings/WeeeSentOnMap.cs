namespace EA.Weee.RequestHandlers.Mappings
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain.AatfReturn;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class WeeeSentOnMap : IMap<WeeeSentOn, WeeeSentOnData>
    {
        private readonly IMap<AatfAddress, AatfAddressData> aatfAddressMapper;

        public WeeeSentOnMap(IMap<AatfAddress, AatfAddressData> aatfAddressMapper)
        {
            this.aatfAddressMapper = aatfAddressMapper;
        }

        public WeeeSentOnData Map(WeeeSentOn source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var weeeSentOn = new WeeeSentOnData()
            {
                SiteAddress = aatfAddressMapper.Map(source.SiteAddress),
                OperatorAddress = aatfAddressMapper.Map(source.OperatorAddress),
                ReturnId = source.ReturnId,
                AatfId = source.AatfId,
                SiteAddressId = source.SiteAddressId,
                WeeeSentOnId = source.Id
            };

            return weeeSentOn;
        }
    }
}
