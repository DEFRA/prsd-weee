namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using System;
    using System.Collections.Generic;
    using EA.Weee.Core.AatfReturn;

    public class ReusedOffSiteSummaryListViewModel
    {
        public Guid OrganisationId { get; set; }

        public Guid ReturnId { get; set; }

        public Guid AatfId { get; set; }

        public string B2cTotal { get; set; }

        public string B2bTotal { get; set; }

        public List<SiteAddressData> Addresses { get; set; }

        public ReusedOffSiteSummaryListViewModel()
        {
        }

        public ReusedOffSiteSummaryListViewModel(List<AddressDataSummary> addresses)
        {
        }

        public string CreateLongAddress(SiteAddressData address)
        {
            string siteAddressLong = address.Address1;

            if (address.Address2 != null)
            {
                siteAddressLong += ",<br/>" + address.Address2;
            }

            siteAddressLong += ",<br/>" + address.TownOrCity;

            if (address.CountyOrRegion != null)
            {
                siteAddressLong += ",<br/>" + address.CountyOrRegion;
            }

            if (address.Postcode != null)
            {
                siteAddressLong += ",<br/>" + address.Postcode;
            }

            siteAddressLong += ",<br/>" + address.CountryName;

            return siteAddressLong;
        }
    }
}