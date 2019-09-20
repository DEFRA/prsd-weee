namespace EA.Weee.Domain.Tests.Unit.AatfReturn
{
    using EA.Weee.Domain.AatfReturn;
    using FakeItEasy;
    using FluentAssertions;
    using System;

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
        public void AatfContact_GivenNullOrEmptyRequiredParameters_ThrowsArgumentException(string firstName, string lastName, string position, string address, string town, string telephone, string email)
        {
            Action constructor = () =>
            {
                var @return = new AatfContact(firstName, lastName, position, address, A.Dummy<string>(), town, A.Dummy<string>(), A.Dummy<string>(), A.Dummy<Country>(), telephone, email);
            };

            constructor.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void AatfAddress_GivenFirstNameIsGreaterThan35Characters_ThrowsArgumentException()
        {
            Action constructor = () =>
            {
                var @return = new AatfContact(new string('*', 36), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<Country>(), A.Dummy<string>(), A.Dummy<string>());
            };

            constructor.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void AatfAddress_GivenLastNameIsGreaterThan35Characters_ThrowsArgumentException()
        {
            Action constructor = () =>
            {
                var @return = new AatfContact(A.Dummy<string>(), new string('*', 36), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<Country>(), A.Dummy<string>(), A.Dummy<string>());
            };

            constructor.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void AatfAddress_GivenPositionIsGreaterThan60Characters_ThrowsArgumentException()
        {
            Action constructor = () =>
            {
                var @return = new AatfContact(A.Dummy<string>(), A.Dummy<string>(), new string('*', 36), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<Country>(), A.Dummy<string>(), A.Dummy<string>());
            };

            constructor.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void AatfAddress_GivenAddress1IsGreaterThan60Characters_ThrowsArgumentException()
        {
            Action constructor = () =>
            {
                var @return = new AatfContact(A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), new string('*', 61), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<Country>(), A.Dummy<string>(), A.Dummy<string>());
            };

            constructor.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void AatfAddress_GivenAddress2IsGreaterThan60Characters_ThrowsArgumentException()
        {
            Action constructor = () =>
            {
                var @return = new AatfContact(A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), new string('*', 61), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<Country>(), A.Dummy<string>(), A.Dummy<string>());
            };

            constructor.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void AatfAddress_GivenTownOrCityIsGreaterThan35Characters_ThrowsArgumentException()
        {
            Action constructor = () =>
            {
                var @return = new AatfContact(A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), new string('*', 36), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<Country>(), A.Dummy<string>(), A.Dummy<string>());
            };

            constructor.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void AatfAddress_GivenCountyOrRegionIsGreaterThan35Characters_ThrowsArgumentException()
        {
            Action constructor = () =>
            {
                var @return = new AatfContact(A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), new string('*', 36), A.Dummy<string>(), A.Dummy<Country>(), A.Dummy<string>(), A.Dummy<string>());
            };

            constructor.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void AatfAddress_GivenPostcodeIsGreaterThan10Characters_ThrowsArgumentException()
        {
            Action constructor = () =>
            {
                var @return = new AatfContact(A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), new string('*', 11), A.Dummy<Country>(), A.Dummy<string>(), A.Dummy<string>());
            };

            constructor.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void AatfAddress_GivenCountryIsNull_ThrowsArgumentException()
        {
            Action constructor = () =>
            {
                var @return = new AatfContact(A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), null, A.Dummy<string>(), A.Dummy<string>());
            };

            constructor.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void AatfAddress_GivenTelephoneIsGreaterThan21Characters_ThrowsArgumentException()
        {
            Action constructor = () =>
            {
                var @return = new AatfContact(A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<Country>(), new string('*', 21), A.Dummy<string>());
            };

            constructor.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void AatfAddress_GivenEmailIsGreaterThan256Characters_ThrowsArgumentException()
        {
            Action constructor = () =>
            {
                var @return = new AatfContact(A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<Country>(), A.Dummy<string>(), new string('*', 257));
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

        [Fact]
        public void Equals_GivenSameContact_TrueShouldBeReturned()
        {
            var countryId = Guid.NewGuid();

            var contact = new AatfContact("f", "l", "p", "a1", "a2", "t", "c", "p", countryId, "te", "e");
            var otherContact = new AatfContact("f", "l", "p", "a1", "a2", "t", "c", "p", countryId, "te", "e");

            contact.Equals(otherContact).Should().BeTrue();
        }

        [Fact]
        public void Equals_GivenDifferentFirstName_FalseShouldBeReturned()
        {
            var countryId = Guid.NewGuid();

            var contact = new AatfContact("Z", "l", "p", "a1", "a2", "t", "c", "p", countryId, "te", "e");
            var otherContact = new AatfContact("f", "l", "p", "a1", "a2", "t", "c", "p", countryId, "te", "e");

            contact.Equals(otherContact).Should().BeFalse();
        }

        [Fact]
        public void Equals_GivenDifferentLastName_FalseShouldBeReturned()
        {
            var countryId = Guid.NewGuid();

            var contact = new AatfContact("f", "z", "p", "a1", "a2", "t", "c", "p", countryId, "te", "e");
            var otherContact = new AatfContact("f", "l", "p", "a1", "a2", "t", "c", "p", countryId, "te", "e");

            contact.Equals(otherContact).Should().BeFalse();
        }

        [Fact]
        public void Equals_GivenDifferentPosition_FalseShouldBeReturned()
        {
            var countryId = Guid.NewGuid();

            var contact = new AatfContact("f", "l", "z", "a1", "a2", "t", "c", "p", countryId, "te", "e");
            var otherContact = new AatfContact("f", "l", "p", "a1", "a2", "t", "c", "p", countryId, "te", "e");

            contact.Equals(otherContact).Should().BeFalse();
        }

        [Fact]
        public void Equals_GivenDifferentAddress1_FalseShouldBeReturned()
        {
            var countryId = Guid.NewGuid();

            var contact = new AatfContact("f", "l", "p", "z", "a2", "t", "c", "p", countryId, "te", "e");
            var otherContact = new AatfContact("f", "l", "p", "a1", "a2", "t", "c", "p", countryId, "te", "e");

            contact.Equals(otherContact).Should().BeFalse();
        }

        [Fact]
        public void Equals_GivenDifferentAddress2_FalseShouldBeReturned()
        {
            var countryId = Guid.NewGuid();

            var contact = new AatfContact("f", "l", "p", "a1", "z", "t", "c", "p", countryId, "te", "e");
            var otherContact = new AatfContact("f", "l", "p", "a1", "a2", "t", "c", "p", countryId, "te", "e");

            contact.Equals(otherContact).Should().BeFalse();
        }

        [Fact]
        public void Equals_GivenDifferentTown_FalseShouldBeReturned()
        {
            var countryId = Guid.NewGuid();

            var contact = new AatfContact("f", "l", "p", "a1", "a2", "z", "c", "p", countryId, "te", "e");
            var otherContact = new AatfContact("f", "l", "p", "a1", "a2", "t", "c", "p", countryId, "te", "e");

            contact.Equals(otherContact).Should().BeFalse();
        }

        [Fact]
        public void Equals_GivenDifferentCounty_FalseShouldBeReturned()
        {
            var countryId = Guid.NewGuid();

            var contact = new AatfContact("f", "l", "p", "a1", "a2", "t", "z", "p", countryId, "te", "e");
            var otherContact = new AatfContact("f", "l", "p", "a1", "a2", "t", "c", "p", countryId, "te", "e");

            contact.Equals(otherContact).Should().BeFalse();
        }

        [Fact]
        public void Equals_GivenDifferentPostcode_FalseShouldBeReturned()
        {
            var countryId = Guid.NewGuid();

            var contact = new AatfContact("f", "l", "p", "a1", "a2", "t", "c", "z", countryId, "te", "e");
            var otherContact = new AatfContact("f", "l", "p", "a1", "a2", "t", "c", "p", countryId, "te", "e");

            contact.Equals(otherContact).Should().BeFalse();
        }

        [Fact]
        public void Equals_GivenDifferentCountry_FalseShouldBeReturned()
        {
            var countryId = Guid.NewGuid();

            var contact = new AatfContact("f", "l", "p", "a1", "a2", "t", "c", "p", Guid.NewGuid(), "te", "e");
            var otherContact = new AatfContact("f", "l", "p", "a1", "a2", "t", "c", "p", countryId, "te", "e");

            contact.Equals(otherContact).Should().BeFalse();
        }

        [Fact]
        public void Equals_GivenDifferentTelephone_FalseShouldBeReturned()
        {
            var countryId = Guid.NewGuid();

            var contact = new AatfContact("f", "l", "p", "a1", "a2", "t", "c", "p", countryId, "z", "e");
            var otherContact = new AatfContact("f", "l", "p", "a1", "a2", "t", "c", "p", countryId, "te", "e");

            contact.Equals(otherContact).Should().BeFalse();
        }

        [Fact]
        public void Equals_GivenDifferentEmail_FalseShouldBeReturned()
        {
            var countryId = Guid.NewGuid();

            var contact = new AatfContact("f", "l", "p", "a1", "a2", "t", "c", "p", countryId, "te", "z");
            var otherContact = new AatfContact("f", "l", "p", "a1", "a2", "t", "c", "p", countryId, "te", "e");

            contact.Equals(otherContact).Should().BeFalse();
        }
    }
}
