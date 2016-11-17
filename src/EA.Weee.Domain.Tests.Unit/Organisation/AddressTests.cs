namespace EA.Weee.Domain.Tests.Unit.Organisation
{
    using Domain.Organisation;
    using Xunit;

    public class AddressTests
    {
        [Fact]
        public void Address_EqualsNullParameter_ReturnsFalse()
        {
            var address = AddressBuilder.NewAddress;

            Assert.NotEqual(address, null);
        }

        [Fact]
        public void Address_EqualsObjectParameter_ReturnsFalse()
        {
            var address = AddressBuilder.NewAddress;

            Assert.NotEqual(address, new object());
        }

        [Fact]
        public void Address_EqualsSameInstance_ReturnsTrue()
        {
            var address = AddressBuilder.NewAddress;

            Assert.Equal(address, address);
        }

        [Fact]
        public void Address_EqualsAddressSameDetails_ReturnsTrue()
        {
            var address = AddressBuilder.NewAddress;
            var address2 = AddressBuilder.NewAddress;

            Assert.Equal(address, address2);
        }

        [Fact]
        public void Address_EqualsAddressDifferentAddress1_ReturnsFalse()
        {
            var address = AddressBuilder.NewAddress;
            var address2 = AddressBuilder.WithAddress1("test address 1");

            Assert.NotEqual(address, address2);
        }

        [Fact]
        public void Address_EqualsAddressDifferentAddress2_ReturnsFalse()
        {
            var address = AddressBuilder.NewAddress;
            var address2 = AddressBuilder.WithAddress1("test address 2");

            Assert.NotEqual(address, address2);
        }

        [Fact]
        public void Address_EqualsAddressDifferentTownOrCity_ReturnsFalse()
        {
            var address = AddressBuilder.NewAddress;
            var address2 = AddressBuilder.WithTownOrCity("test town or city");

            Assert.NotEqual(address, address2);
        }

        [Fact]
        public void Address_EqualsAddressDifferentCountyOrRegion_ReturnsFalse()
        {
            var address = AddressBuilder.NewAddress;
            var address2 = AddressBuilder.WithCountyOrRegion("test county or region");

            Assert.NotEqual(address, address2);
        }

        [Fact]
        public void Address_EqualsAddressDifferentPostcode_ReturnsFalse()
        {
            var address = AddressBuilder.NewAddress;
            var address2 = AddressBuilder.WithPostcode("Postcode2");

            Assert.NotEqual(address, address2);
        }

        [Fact]
        public void Address_EqualsAddressDifferentTelephone_ReturnsFalse()
        {
            var address = AddressBuilder.NewAddress;
            var address2 = AddressBuilder.WithTelephone("test Telephone");

            Assert.NotEqual(address, address2);
        }

        [Fact]
        public void Address_EqualsAddressDifferentEmail_ReturnsFalse()
        {
            var address = AddressBuilder.NewAddress;
            var address2 = AddressBuilder.WithEmail("test Email");

            Assert.NotEqual(address, address2);
        }

        [Fact]
        public void Address_EqualsAddressDifferentCountry_ReturnsFalse()
        {
            var address = AddressBuilder.WithCountry(new AlwaysUnequalCountry());
            var address2 = AddressBuilder.NewAddress;

            Assert.NotEqual(address, address2);
        }

        private class AddressBuilder
        {
            private string Address1 { get; set; }
            private string Address2 { get; set; }
            private string TownOrCity { get; set; }
            private string CountyOrRegion { get; set; }
            private string Postcode { get; set; }
            private string Telephone { get; set; }
            private string Email { get; set; }

            public Country Country { get; set; }

            public AddressBuilder()
            {
                Address1 = "Address1";
                Address2 = "Address2";
                TownOrCity = "TownOrCity";
                CountyOrRegion = "CountyOrRegion";
                Postcode = "PostCode";
                Telephone = "Telephone";
                Email = "Email";
                Country = new AlwaysEqualCountry();
            }

            public Address Build()
            {
                return new Address(Address1, Address2, TownOrCity, CountyOrRegion,
                    Postcode, Country, Telephone, Email);
            }

            public static Address NewAddress
            {
                get { return new AddressBuilder().Build(); }
            }

            public static Address WithAddress1(string address1)
            {
                var builder = new AddressBuilder();
                builder.Address1 = address1;

                return builder.Build();
            }

            public static Address WithAddress2(string address2)
            {
                var builder = new AddressBuilder();
                builder.Address2 = address2;

                return builder.Build();
            }

            public static Address WithTownOrCity(string townOrCity)
            {
                var builder = new AddressBuilder();
                builder.TownOrCity = townOrCity;

                return builder.Build();
            }

            public static Address WithCountyOrRegion(string countyOrRegion)
            {
                var builder = new AddressBuilder();
                builder.CountyOrRegion = countyOrRegion;

                return builder.Build();
            }

            public static Address WithPostcode(string postcode)
            {
                var builder = new AddressBuilder();
                builder.Postcode = postcode;

                return builder.Build();
            }

            public static Address WithTelephone(string telephone)
            {
                var builder = new AddressBuilder();
                builder.Telephone = telephone;

                return builder.Build();
            }

            public static Address WithEmail(string email)
            {
                var builder = new AddressBuilder();
                builder.Email = email;

                return builder.Build();
            }

            public static Address WithCountry(Country country)
            {
                var builder = new AddressBuilder();
                builder.Country = country;

                return builder.Build();
            }
        }

        private class AlwaysEqualCountry : Country
        {
            public override bool Equals(Country other)
            {
                return true;
            }
        }

        public class AlwaysUnequalCountry : Country
        {
            public override bool Equals(Country other)
            {
                return false;
            }
        }
    }
}
