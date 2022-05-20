namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using EA.Weee.Core.AatfReturn;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class SearchedAatfResultListViewModel
    {
        public Guid OrganisationId { get; set; }

        public Guid ReturnId { get; set; }

        public Guid AatfId { get; set; }

        public string AatfName { get; set; }

        public List<WeeeSearchedAnAatfListData> Sites { get; set; }

        [Required(ErrorMessage = "You must select a scheme to manage")]
        public Guid? SelectedWeeeSentOnId { get; set; }

        public Guid SelectedAatfId { get; set; }

        public string SelectedAatfName { get; set; }

        public string SelectedSiteName { get; set; }

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