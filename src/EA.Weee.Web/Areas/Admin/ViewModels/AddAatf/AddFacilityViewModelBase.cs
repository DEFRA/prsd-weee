namespace EA.Weee.Web.Areas.Admin.ViewModels.AddAatf
{
    using System;
    using System.Collections.Generic;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.Admin.ViewModels.Aatf;

    public abstract class AddFacilityViewModelBase : FacilityViewModelBase
    {
        public Guid OrganisationId { get; set; }

        public string OrganisationName { get; set; }

        public AatfContactData ContactData { get; set; }

        public IEnumerable<short> ComplianceYearList => new List<short> { 2019 };

        public static new IEnumerable<string> ValidationMessageDisplayOrder => new List<string>
        {
            nameof(Name),
            $"{nameof(SiteAddressData)}.{nameof(AatfAddressData.Address1)}",
            $"{nameof(SiteAddressData)}.{nameof(AatfAddressData.Address2)}",
            $"{nameof(SiteAddressData)}.{nameof(AatfAddressData.TownOrCity)}",
            $"{nameof(SiteAddressData)}.{nameof(AatfAddressData.CountyOrRegion)}",
            $"{nameof(SiteAddressData)}.{nameof(AatfAddressData.Postcode)}",
            $"{nameof(SiteAddressData)}.{nameof(AatfAddressData.CountryId)}",
            nameof(ApprovalNumber),
            nameof(CompetentAuthorityId),
            nameof(StatusValue),
            nameof(SizeValue),
            nameof(ApprovalDate),
            nameof(ComplianceYear),
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

        public AddFacilityViewModelBase()
        {
            ContactData = new AatfContactData();
            SiteAddressData = new AatfAddressData();
        }
    }
}