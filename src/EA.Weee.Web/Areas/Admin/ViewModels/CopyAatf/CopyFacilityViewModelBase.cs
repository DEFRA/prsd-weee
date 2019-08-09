namespace EA.Weee.Web.Areas.Admin.ViewModels.CopyAatf
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.Admin.ViewModels.Aatf;

    public abstract class CopyFacilityViewModelBase : FacilityViewModelBase
    {        
        public IEnumerable<SelectListItem> ComplianceYearList
        {
            get
            {
                yield return new SelectListItem() { Text = "2020" };
                yield return new SelectListItem() { Text = "2021" };                
            }
        }

        public AatfContactData ContactData { get; set; }

        public Guid OrganisationId { get; set; }

        public static new IEnumerable<string> ValidationMessageDisplayOrder => new List<string>
        {
            nameof(ComplianceYear),
            nameof(Name),
            $"{nameof(SiteAddressData)}.{nameof(AatfAddressData.Address1)}",
            $"{nameof(SiteAddressData)}.{nameof(AatfAddressData.Address2)}",
            $"{nameof(SiteAddressData)}.{nameof(AatfAddressData.TownOrCity)}",
            $"{nameof(SiteAddressData)}.{nameof(AatfAddressData.CountyOrRegion)}",
            $"{nameof(SiteAddressData)}.{nameof(AatfAddressData.Postcode)}",
            $"{nameof(SiteAddressData)}.{nameof(AatfAddressData.CountryId)}",
            nameof(ApprovalNumber),
            nameof(CompetentAuthorityId),
            nameof(PanAreaId),
            nameof(LocalAreaId),
            nameof(StatusValue),
            nameof(SizeValue),
            nameof(ApprovalDate),            
            $"{nameof(ContactData)}.{nameof(AatfContactData.FirstName)}",
            $"{nameof(ContactData)}.{nameof(AatfContactData.LastName)}",
            $"{nameof(ContactData)}.{nameof(AatfContactData.Position)}",
            $"{nameof(ContactData)}.{nameof(AatfContactData.AddressData)}.{nameof(AatfContactAddressData.Address1)}",
            $"{nameof(ContactData)}.{nameof(AatfContactData.AddressData)}.{nameof(AatfContactAddressData.Address2)}",
            $"{nameof(ContactData)}.{nameof(AatfContactData.AddressData)}.{nameof(AatfContactAddressData.TownOrCity)}",
            $"{nameof(ContactData)}.{nameof(AatfContactData.AddressData)}.{nameof(AatfContactAddressData.CountyOrRegion)}",
            $"{nameof(ContactData)}.{nameof(AatfContactData.AddressData)}.{nameof(AatfContactAddressData.Postcode)}",
            $"{nameof(ContactData)}.{nameof(AatfContactData.AddressData)}.{nameof(AatfContactAddressData.CountryId)}",
            $"{nameof(ContactData)}.{nameof(AatfContactData.Telephone)}",
            $"{nameof(ContactData)}.{nameof(AatfContactData.Email)}"
        };
    }
}