namespace EA.Weee.Domain.Tests.Unit.AatfReturn
{
    using System;
    using EA.Weee.Domain.AatfReturn;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class AatfContactTests
    {
        [Theory]
        [InlineData("", "-", "-", "-", "-", "-", "-")]
        [InlineData(null, "-", "-", "-", "-", "-", "-")]

        [InlineData("-", "", "-", "-", "-", "-", "-")]
        [InlineData("-", null, "-", "-", "-", "-", "-")]

        [InlineData("-", "-", "", "-", "-", "-", "-")]
        [InlineData("-", "-", null, "-", "-", "-", "-")]

        [InlineData("-", "-", "-", "", "-", "-", "-")]
        [InlineData("-", "-", "-", null, "-", "-", "-")]

        [InlineData("-", "-", "-", "-", "", "-", "-")]
        [InlineData("-", "-", "-", "-", null, "-", "-")]

        [InlineData("-", "-", "-", "-", "-", "", "-")]
        [InlineData("-", "-", "-", "-", "-", null, "-")]

        [InlineData("-", "-", "-", "-", "-", "-", "")]
        [InlineData("-", "-", "-", "-", "-", "-", null)]
        public void AatfContact_GivenNullOrEmptyRequiredParameters_ExceptionThrown(string firstName, string lastName, string position, string address, string town, string telephone, string email)
        {
            Action constructor = () =>
            {
                var @return = new AatfContact(firstName, lastName, position, address, A.Dummy<string>(), town, A.Dummy<string>(), A.Dummy<string>(), A.Dummy<Country>(), telephone, email);
            };

            constructor.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void AatfContact_GivenValidParameters_AatfContactPropertiesShouldBeSet()
        {
            var firstName = "First Name";
            var lastName = "Last Name";
            var position = "Position";
            var address1 = "Address1";
            var address2 = "Address2";
            var town = "Town";
            var county = "County";
            var postcode = "Postcode";
            var country = new Country(Guid.NewGuid(), "Country");
            var telephone = "Telephone";
            var email = "Email";

            var contact = new AatfContact(firstName, lastName, position, address1, address2, town, county, postcode, country, telephone, email);

            contact.FirstName.Should().Be(firstName);
            contact.LastName.Should().Be(lastName);
            contact.Position.Should().Be(position);
            contact.Address1.Should().Be(address1);
            contact.Address2.Should().Be(address2);
            contact.TownOrCity.Should().Be(town);
            contact.CountyOrRegion.Should().Be(county);
            contact.Postcode.Should().Be(postcode);
            contact.Country.Should().Be(country);
            contact.Telephone.Should().Be(telephone);
            contact.Email.Should().Be(email);
        }
    }
}
