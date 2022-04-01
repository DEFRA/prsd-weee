namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using EA.Weee.Core.AatfReturn;
    using System;
    using System.Collections.Generic;

    public class SearchedAatfResultListViewModel
    {
        public Guid OrganisationId { get; set; }

        public Guid ReturnId { get; set; }

        public Guid AatfId { get; set; }

        public String AatfName { get; set; }

        public List<WeeeSearchedAnAatfListData> Sites { get; set; }

        public SearchedAatfResultListViewModel()
        {
        }

        public string CreateLongAddress(AatfAddressData address)
        {
            if (address != null)
            {
                string siteAddressLong = address.Name + ",<br/>" + address.Address1;

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
            else
            {
                return "&nbsp";
            }
        }
    }
}