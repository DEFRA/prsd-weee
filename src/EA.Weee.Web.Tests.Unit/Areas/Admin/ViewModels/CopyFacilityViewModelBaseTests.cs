namespace EA.Weee.Web.Tests.Unit.Areas.Admin.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using EA.Weee.Web.Areas.Admin.ViewModels.CopyAatf;
    using Xunit;

    public class CopyFacilityViewModelBaseTests
    {
        [Fact]
        public void ValidationMessageDisplayOrder_IsAsExpected()
        {
            var expectedOrdering = new List<string>
            {
                "ComplianceYear",
                "Name",
                "SiteAddressData.Address1",
                "SiteAddressData.Address2",
                "SiteAddressData.TownOrCity",
                "SiteAddressData.CountyOrRegion",
                "SiteAddressData.Postcode",
                "SiteAddressData.CountryId",
                "ApprovalNumber",
                "CompetentAuthorityId",
                "PanAreaId",
                "LocalAreaId",
                "StatusValue",
                "SizeValue",
                "ApprovalDate",
                "ContactData.FirstName",
                "ContactData.LastName",
                "ContactData.Position",
                "ContactData.AddressData.Address1",
                "ContactData.AddressData.Address2",
                "ContactData.AddressData.TownOrCity",
                "ContactData.AddressData.CountyOrRegion",
                "ContactData.AddressData.Postcode",
                "ContactData.AddressData.CountryId",
                "ContactData.Telephone",
                "ContactData.Email"
            };

            var actualOrdering = CopyFacilityViewModelBase.ValidationMessageDisplayOrder.ToList();

            Assert.Equal(expectedOrdering.Count, actualOrdering.Count);
            Assert.Equal(expectedOrdering, actualOrdering);
        }
    }
}
