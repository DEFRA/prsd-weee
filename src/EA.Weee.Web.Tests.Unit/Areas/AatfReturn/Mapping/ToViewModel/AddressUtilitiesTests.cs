namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Mapping.ToViewModel
{
    using EA.Weee.Core.AatfReturn;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using AutoFixture;
    using Core.Shared;
    using Xunit;

    public class AddressUtilitiesTests
    {
        private readonly AddressUtilities addressUtilities;

        public AddressUtilitiesTests()
        {
            addressUtilities = new AddressUtilities();
        }

        [Fact]
        public void AddressConcatenate_OnAddressData_ReturnsCorrectlyFormattedAddress()
        {
            var addressData = new SiteAddressData(
                "Name",
                "Address1",
                "Address2",
                "Town",
                "County",
                "PO12 3ST",
                A.Dummy<Guid>(),
                "Country");

            var result = addressUtilities.AddressConcatenate(addressData);

            result.Should().Be("Address1, Address2, Town, County, PO12 3ST, Country");
        }

        [Fact]
        public void AddressConcatenate_OnAddressDataWithAllowedNulls_ReturnsCorrectlyFormattedAddress()
        {
            var addressData = new SiteAddressData(
                "Name",
                "Address1",
                null,
                "Town",
                null,
                null,
                A.Dummy<Guid>(),
                "Country");

            var result = addressUtilities.AddressConcatenate(addressData);

            result.Should().Be("Address1, Town, Country");
        }

        [Fact]
        public void StringConcatenate_OnInputs_ReturnsCorrectlyFormattedString()
        {
            var result = addressUtilities.StringConcatenate("My Current Address, To Be Checked", "Element to Add");

            result.Should().Be("My Current Address, To Be Checked, Element to Add");
        }

        [Fact]
        public void FormattedAddress_GivenFullAddress_AddressShouldBeFormattedCorrectly()
        {
            var result = addressUtilities.FormattedAddress(new AatfAddressData("Site name", "Site address 1", "Site address 2", "Site town", "Site county", "GU22 7UY", Guid.NewGuid(), "Site country"));

            result.Should().Be("Site name<br/>Site address 1<br/>Site address 2<br/>Site town<br/>Site county<br/>GU22 7UY<br/>Site country");
        }

        [Fact]
        public void FormattedAddress_GivenFullAddressWithoutSiteName_AddressShouldBeFormattedCorrectly()
        {
            var result = addressUtilities.FormattedAddress(new AatfAddressData("Site name", "Site address 1", "Site address 2", "Site town", "Site county", "GU22 7UY", Guid.NewGuid(), "Site country"), false);

            result.Should().Be("Site address 1<br/>Site address 2<br/>Site town<br/>Site county<br/>GU22 7UY<br/>Site country");
        }

        [Fact]
        public void FormattedAddress_GivenAddressWithoutAddress2_AddressShouldBeFormattedCorrectly()
        {
            var result = addressUtilities.FormattedAddress(new AatfAddressData("Site name", "Site address 1", null, "Site town", "Site county", "GU22 7UY", Guid.NewGuid(), "Site country"));

            result.Should().Be("Site name<br/>Site address 1<br/>Site town<br/>Site county<br/>GU22 7UY<br/>Site country");
        }

        [Fact]
        public void FormattedAddress_GivenAddressWithoutCounty_AddressShouldBeFormattedCorrectly()
        {
            var result = addressUtilities.FormattedAddress(new AatfAddressData("Site name", "Site address 1", "Site address 2", "Site town", null, "GU22 7UY", Guid.NewGuid(), "Site country"));

            result.Should().Be("Site name<br/>Site address 1<br/>Site address 2<br/>Site town<br/>GU22 7UY<br/>Site country");
        }

        [Fact]
        public void FormattedAddress_GivenAddressWithoutPostcode_AddressShouldBeFormattedCorrectly()
        {
            var result = addressUtilities.FormattedAddress(new AatfAddressData("Site name", "Site address 1", "Site address 2", "Site town", "Site county", null, Guid.NewGuid(), "Site country"));

            result.Should().Be("Site name<br/>Site address 1<br/>Site address 2<br/>Site town<br/>Site county<br/>Site country");
        }

        [Fact]
        public void FormattedAddress_GivenAddressWithoutAnyOptionFields_AddressShouldBeFormattedCorrectly()
        {
            var result = addressUtilities.FormattedAddress(new AatfAddressData("Site name", "Site address 1", null, "Site town", null, null, Guid.NewGuid(), "Site country"));

            result.Should().Be("Site name<br/>Site address 1<br/>Site town<br/>Site country");
        }

        [Fact]
        public void FormattedAddress_GivenAddressIsNull_EmptyAddressExpected()
        {
            var result = addressUtilities.FormattedAddress(null);

            result.Should().BeEmpty();
        }

        [Fact]
        public void FormattedAddress2_GivenAddressFields_AddressShouldBeFormattedCorrectly()
        {
            var name = Faker.Company.Name();
            var address1 = Faker.Address.StreetName();
            var address2 = Faker.Address.StreetAddress();
            var town = Faker.Address.City();
            var county = Faker.Address.UkCounty();
            var postcode = Faker.Address.UkPostCode();
            var approval = Faker.Name.Suffix();

            var result = addressUtilities.FormattedAddress(name, address1, address2, town, county, postcode, approval);

            result.Should().Be($@"<span>{name}</span><strong><span>{approval}</span></strong><span>{address1}</span><span>{address2}</span><span>{town}</span><span>{county}</span><span>{postcode}</span>");
        }

        [Fact]
        public void FormattedAddress2_GivenAddressFieldsWithNoApprovalNumber_AddressShouldBeFormattedCorrectly()
        {
            var name = Faker.Company.Name();
            var address1 = Faker.Address.StreetName();
            var address2 = Faker.Address.StreetAddress();
            var town = Faker.Address.City();
            var county = Faker.Address.UkCounty();
            var postcode = Faker.Address.UkPostCode();

            var result = addressUtilities.FormattedAddress(name, address1, address2, town, county, postcode);

            result.Should().Be($@"<span>{name}</span><span>{address1}</span><span>{address2}</span><span>{town}</span><span>{county}</span><span>{postcode}</span>");
        }

        [Fact]
        public void FormattedAddress2_GivenAddressFieldsWithNoAddress2_AddressShouldBeFormattedCorrectly()
        {
            var name = Faker.Company.Name();
            var address1 = Faker.Address.StreetName();
            var town = Faker.Address.City();
            var county = Faker.Address.UkCounty();
            var postcode = Faker.Address.UkPostCode();
            var approval = Faker.Name.Suffix();

            var result = addressUtilities.FormattedAddress(name, address1, null, town, county, postcode, approval);

            result.Should().Be($@"<span>{name}</span><strong><span>{approval}</span></strong><span>{address1}</span><span>{town}</span><span>{county}</span><span>{postcode}</span>");
        }

        [Fact]
        public void FormattedAddress2_GivenAddressFieldsWithNoApprovalOrAddress2_AddressShouldBeFormattedCorrectly()
        {
            var name = Faker.Company.Name();
            var address1 = Faker.Address.StreetName();
            var town = Faker.Address.City();
            var county = Faker.Address.UkCounty();
            var postcode = Faker.Address.UkPostCode();

            var result = addressUtilities.FormattedAddress(name, address1, null, town, county, postcode);

            result.Should().Be($@"<span>{name}</span><span>{address1}</span><span>{town}</span><span>{county}</span><span>{postcode}</span>");
        }

        [Theory]
        [InlineData("test", "test", "test")]
        [InlineData("TEST", "test", "test")]
        [InlineData(" Test ", "test", "test")]
        [InlineData("No", "Match", "No</span><span>Match")]
        public void FormattedAddress3_ShouldIgnoreSpacesAndSpecialSymbolsAndCase(string companyName, string name, string expectedNameString)
        {
            var address1 = Faker.Address.StreetName();
            var town = Faker.Address.City();
            var county = Faker.Address.UkCounty();
            var postcode = Faker.Address.UkPostCode();

            var result = addressUtilities.FormattedCompanyPcsAddress(companyName, name, address1, null, town, county, postcode, null);

            result.Should().Be($@"<span>{expectedNameString}</span><span>{address1}</span><span>{town}</span><span>{county}</span><span>{postcode}</span>");
        }

        [Fact]
        public void FormattedAddress3_ShouldIgnoreSpecialChars()
        {
            var companyName = @"""!£$%^&*()_+-=`¬TEST";
            var name = @"|\[]{}:;@#~.?/test";

            var address1 = Faker.Address.StreetName();
            var town = Faker.Address.City();
            var county = Faker.Address.UkCounty();
            var postcode = Faker.Address.UkPostCode();

            var result = addressUtilities.FormattedCompanyPcsAddress(companyName, name, address1, null, town, county, postcode, null);

            result.Should().Be($@"<span>{name}</span><span>{address1}</span><span>{town}</span><span>{county}</span><span>{postcode}</span>");
        }

        [Fact]
        public void FormattedAddress3_ShouldAddCompanyName_OnNonMatch()
        {
            var companyName = Faker.Company.Name();
            var name = Faker.Company.Name();
            var address1 = Faker.Address.StreetName();
            var town = Faker.Address.City();
            var county = Faker.Address.UkCounty();
            var postcode = Faker.Address.UkPostCode();

            var result = addressUtilities.FormattedCompanyPcsAddress(companyName, name, address1, null, town, county, postcode, null);

            result.Should().Be($@"<span>{companyName}</span><span>{name}</span><span>{address1}</span><span>{town}</span><span>{county}</span><span>{postcode}</span>");
        }

        [Fact]
        public void FormattedAddress3_ShouldNotAddCompanyName_OnMatch()
        {
            var companyName = "Test";
            var name = "Test";
            var address1 = Faker.Address.StreetName();
            var town = Faker.Address.City();
            var county = Faker.Address.UkCounty();
            var postcode = Faker.Address.UkPostCode();

            var result = addressUtilities.FormattedCompanyPcsAddress(companyName, name, address1, null, town, county, postcode, null);

            result.Should().Be($@"<span>{name}</span><span>{address1}</span><span>{town}</span><span>{county}</span><span>{postcode}</span>");
        }
    }
}