namespace EA.Weee.Domain.Tests.Unit.Producer
{
    using System;
    using EA.Weee.Domain.Producer;
    using Xunit;

    public class ProducerAddressTests
    {
        [Fact]
        public void ProducerAddress_EqualsNullParameter_ReturnsFalse()
        {
            var producerAddress = ProducerAddressBuilder.NewProducerAddress;

            Assert.NotEqual(producerAddress, null);
        }

        [Fact]
        public void ProducerAddress_EqualsObjectParameter_ReturnsFalse()
        {
            var producerAddress = ProducerAddressBuilder.NewProducerAddress;

            Assert.NotEqual(producerAddress, new object());
        }

        [Fact]
        public void ProducerAddress_EqualsSameInstance_ReturnsTrue()
        {
            var producerAddress = ProducerAddressBuilder.NewProducerAddress;

            Assert.Equal(producerAddress, producerAddress);
        }

        [Fact]
        public void ProducerAddress_EqualsProducerAddressSameDetails_ReturnsTrue()
        {
            var producerAddress = ProducerAddressBuilder.NewProducerAddress;
            var producerAddress2 = ProducerAddressBuilder.NewProducerAddress;

            Assert.Equal(producerAddress, producerAddress2);
        }

        [Fact]
        public void ProducerAddress_EqualsProducerAddressDifferentPrimaryName_ReturnsFalse()
        {
            var producerAddress = ProducerAddressBuilder.NewProducerAddress;
            var producerAddress2 = ProducerAddressBuilder.WithPrimaryName("primary name test");

            Assert.NotEqual(producerAddress, producerAddress2);
        }

        [Fact]
        public void ProducerAddress_EqualsProducerAddressDifferentSecondaryName_ReturnsFalse()
        {
            var producerAddress = ProducerAddressBuilder.NewProducerAddress;
            var producerAddress2 = ProducerAddressBuilder.WithSecondaryName("secondary name test");

            Assert.NotEqual(producerAddress, producerAddress2);
        }

        [Fact]
        public void ProducerAddress_EqualsProducerAddressDifferentStreet_ReturnsFalse()
        {
            var producerAddress = ProducerAddressBuilder.NewProducerAddress;
            var producerAddress2 = ProducerAddressBuilder.WithStreet("street test");

            Assert.NotEqual(producerAddress, producerAddress2);
        }

        [Fact]
        public void ProducerAddress_EqualsProducerAddressDifferentTown_ReturnsFalse()
        {
            var producerAddress = ProducerAddressBuilder.NewProducerAddress;
            var producerAddress2 = ProducerAddressBuilder.WithTown("town test");

            Assert.NotEqual(producerAddress, producerAddress2);
        }

        [Fact]
        public void ProducerAddress_EqualsProducerAddressDifferentLocality_ReturnsFalse()
        {
            var producerAddress = ProducerAddressBuilder.NewProducerAddress;
            var producerAddress2 = ProducerAddressBuilder.WithLocality("locality test");

            Assert.NotEqual(producerAddress, producerAddress2);
        }

        [Fact]
        public void ProducerAddress_EqualsProducerAddressDifferentAdministrativeArea_ReturnsFalse()
        {
            var producerAddress = ProducerAddressBuilder.NewProducerAddress;
            var producerAddress2 = ProducerAddressBuilder.WithAdministrativeArea("administrative area test");

            Assert.NotEqual(producerAddress, producerAddress2);
        }

        [Fact]
        public void ProducerAddress_EqualsProducerAddressDifferentPostCode_ReturnsFalse()
        {
            var producerAddress = ProducerAddressBuilder.NewProducerAddress;
            var producerAddress2 = ProducerAddressBuilder.WithPostCode("post code test");

            Assert.NotEqual(producerAddress, producerAddress2);
        }

        [Fact]
        public void ProducerAddress_EqualsProducerAddressDifferentCountry_ReturnsFalse()
        {
            var producerAddress = ProducerAddressBuilder.WithCountry(new AlwaysUnequalCountry());
            var producerAddress2 = ProducerAddressBuilder.WithCountry(new AlwaysUnequalCountry());

            Assert.NotEqual(producerAddress, producerAddress2);
        }

        [Fact]
        public void ProducerAddress_ToString_ReturnsConcatenatedString()
        {
            var producerAddress = ProducerAddressBuilder.WithCountry(new Country(Guid.NewGuid(), "TestCountry"));

            var address = producerAddress.ToString();

            Assert.Equal("PrimaryName, SecondaryName, Street, Town, Locality, AdministrativeArea, PostCode, TestCountry", address);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void ProducerAddress_ToString_ReturnsConcatenatedStringExcludingEmptyValues(string secondaryAddress)
        {
            var buider = new ProducerAddressBuilder();
            buider.SecondaryName = secondaryAddress;
            buider.Country = new Country(Guid.NewGuid(), "TestCountry");

            var producerAddress = buider.Build();

            var address = producerAddress.ToString();

            Assert.Equal("PrimaryName, Street, Town, Locality, AdministrativeArea, PostCode, TestCountry", address);
        }

        private class AlwaysEqualCountry : Country
        {
            public override bool Equals(Country other)
            {
                return true;
            }
        }

        private class AlwaysUnequalCountry : Country
        {
            public override bool Equals(Country other)
            {
                return false;
            }
        }

        private class ProducerAddressBuilder
        {
            private string primaryName = "PrimaryName";
            public string SecondaryName { get; set; }
            private string street = "Street";
            private string town = "Town";
            private string locality = "Locality";
            private string administrativeArea = "AdministrativeArea";
            private string postCode = "PostCode";
            public Country Country { get; set; }

            public ProducerAddressBuilder()
            {
                SecondaryName = "SecondaryName";
                Country = new AlwaysEqualCountry();
            }

            public ProducerAddress Build()
            {
                return new ProducerAddress(
                    primaryName,
                    SecondaryName,
                    street,
                    town,
                    locality,
                    administrativeArea,
                    Country,
                    postCode);
            }

            public static ProducerAddress NewProducerAddress
            {
                get { return new ProducerAddressBuilder().Build(); }
            }

            public static ProducerAddress WithPrimaryName(string primaryName)
            {
                var builder = new ProducerAddressBuilder();
                builder.primaryName = primaryName;

                return builder.Build();
            }

            public static ProducerAddress WithSecondaryName(string secondaryName)
            {
                var builder = new ProducerAddressBuilder();
                builder.SecondaryName = secondaryName;

                return builder.Build();
            }

            public static ProducerAddress WithStreet(string street)
            {
                var builder = new ProducerAddressBuilder();
                builder.street = street;

                return builder.Build();
            }

            public static ProducerAddress WithTown(string town)
            {
                var builder = new ProducerAddressBuilder();
                builder.town = town;

                return builder.Build();
            }

            public static ProducerAddress WithLocality(string locality)
            {
                var builder = new ProducerAddressBuilder();
                builder.locality = locality;

                return builder.Build();
            }

            public static ProducerAddress WithAdministrativeArea(string administrativeArea)
            {
                var builder = new ProducerAddressBuilder();
                builder.administrativeArea = administrativeArea;

                return builder.Build();
            }

            public static ProducerAddress WithPostCode(string postCode)
            {
                var builder = new ProducerAddressBuilder();
                builder.postCode = postCode;

                return builder.Build();
            }

            public static ProducerAddress WithCountry(Country country)
            {
                var builder = new ProducerAddressBuilder();
                builder.Country = country;

                return builder.Build();
            }
        }
    }
}
