namespace EA.Weee.RequestHandlers.Mappings
{
    using System.Collections.Generic;
    using System.Linq;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain.AatfReturn;
    using Aatf = Core.AatfReturn.AatfData;

    public class AatfSentOnSiteMap : IMap<AatfAddress, AatfAddressData>
    {
        public AatfAddressData Map(AatfAddress source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var siteAddress = new AatfAddressData()
            {
                Address1 = source.Address1,
                Address2 = source.Address2,
                TownOrCity = source.TownOrCity,
                CountryId = source.CountryId,
                CountyOrRegion = source.CountyOrRegion,
                Name = source.Name,
                Postcode = source.Postcode
            };

            return siteAddress;
        }
    }
}
