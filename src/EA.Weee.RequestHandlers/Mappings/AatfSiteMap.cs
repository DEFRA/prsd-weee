﻿namespace EA.Weee.RequestHandlers.Mappings
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain.AatfReturn;
    using System.Collections.Generic;
    using System.Linq;
    using Aatf = Core.AatfReturn.AatfData;

    public class AatfSiteMap : IMap<AatfAddressObligatedAmount, AddressTonnageSummary>
    {
        public AddressTonnageSummary Map(AatfAddressObligatedAmount source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var summaryData = new AddressTonnageSummary();

            if (source.AatfAddresses != null)
            {
                summaryData.AddressData = source.AatfAddresses.Select(n => new SiteAddressData(
                    n.Id,
                    n.Name,
                    n.Address1,
                    n.Address2,
                    n.TownOrCity,
                    n.CountyOrRegion,
                    n.Postcode,
                    n.Country.Id,
                    n.Country.Name)).ToList();
            }
            else
            {
                summaryData.AddressData = new List<SiteAddressData>();
            }

            if (source.WeeeReusedAmounts != null)
            {
                summaryData.ObligatedData = source.WeeeReusedAmounts.Select(n => new WeeeObligatedData(
                    n.Id,
                    null,
                    new Aatf(n.WeeeReused.Aatf.Id, n.WeeeReused.Aatf.Name, n.WeeeReused.Aatf.ApprovalNumber, n.WeeeReused.Aatf.ComplianceYear),
                    n.CategoryId,
                    n.NonHouseholdTonnage,
                    n.HouseholdTonnage)).ToList();
            }

            return summaryData;
        }
    }
}
