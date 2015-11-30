namespace EA.Weee.Domain.Tests.Unit.Producer
{
    using EA.Weee.Domain.Producer;
    using Xunit;

    public class ProducerAddressTests
    {
        [Fact]
        public void ProducerAddress_EqualsNullParameter_ReturnsFalse()
        {
            var producerAddress = ProducerAddresstBuilder.NewProducerAddress;

            Assert.NotEqual(producerAddress, null);
        }

        [Fact]
        public void ProducerAddress_EqualsObjectParameter_ReturnsFalse()
        {
            var producerAddress = ProducerAddresstBuilder.NewProducerAddress;

            Assert.NotEqual(producerAddress, new object());
        }

        [Fact]
        public void ProducerAddress_EqualsSameInstance_ReturnsTrue()
        {
            var producerAddress = ProducerAddresstBuilder.NewProducerAddress;

            Assert.Equal(producerAddress, producerAddress);
        }

        [Fact]
        public void ProducerAddress_EqualsProducerAddressSameDetails_ReturnsTrue()
        {
            var producerAddress = ProducerAddresstBuilder.NewProducerAddress;
            var producerAddress2 = ProducerAddresstBuilder.NewProducerAddress;

            Assert.Equal(producerAddress, producerAddress2);
        }

        [Fact]
        public void ProducerAddress_EqualsProducerAddressDifferentPrimaryName_ReturnsFalse()
        {
            var producerAddress = ProducerAddresstBuilder.NewProducerAddress;
            var producerAddress2 = ProducerAddresstBuilder.WithPrimaryName("primary name test");

            Assert.NotEqual(producerAddress, producerAddress2);
        }

        [Fact]
        public void ProducerAddress_EqualsProducerAddressDifferentSecondaryName_ReturnsFalse()
        {
            var producerAddress = ProducerAddresstBuilder.NewProducerAddress;
            var producerAddress2 = ProducerAddresstBuilder.WithSecondaryName("secondary name test");

            Assert.NotEqual(producerAddress, producerAddress2);
        }

        [Fact]
        public void ProducerAddress_EqualsProducerAddressDifferentStreet_ReturnsFalse()
        {
            var producerAddress = ProducerAddresstBuilder.NewProducerAddress;
            var producerAddress2 = ProducerAddresstBuilder.WithStreet("street test");

            Assert.NotEqual(producerAddress, producerAddress2);
        }

        [Fact]
        public void ProducerAddress_EqualsProducerAddressDifferentTown_ReturnsFalse()
        {
            var producerAddress = ProducerAddresstBuilder.NewProducerAddress;
            var producerAddress2 = ProducerAddresstBuilder.WithTown("town test");

            Assert.NotEqual(producerAddress, producerAddress2);
        }

        [Fact]
        public void ProducerAddress_EqualsProducerAddressDifferentLocality_ReturnsFalse()
        {
            var producerAddress = ProducerAddresstBuilder.NewProducerAddress;
            var producerAddress2 = ProducerAddresstBuilder.WithLocality("locality test");

            Assert.NotEqual(producerAddress, producerAddress2);
        }

        [Fact]
        public void ProducerAddress_EqualsProducerAddressDifferentAdministrativeArea_ReturnsFalse()
        {
            var producerAddress = ProducerAddresstBuilder.NewProducerAddress;
            var producerAddress2 = ProducerAddresstBuilder.WithAdministrativeArea("administrative area test");

            Assert.NotEqual(producerAddress, producerAddress2);
        }

        [Fact]
        public void ProducerAddress_EqualsProducerAddressDifferentPostCode_ReturnsFalse()
        {
            var producerAddress = ProducerAddresstBuilder.NewProducerAddress;
            var producerAddress2 = ProducerAddresstBuilder.WithPostCode("post code test");

            Assert.NotEqual(producerAddress, producerAddress2);
        }

        [Fact]
        public void ProducerAddress_EqualsProducerAddressDifferentCountry_ReturnsFalse()
        {
            var producerAddress = ProducerAddresstBuilder.WithCountry(new AlwaysUnequalCountry());
            var producerAddress2 = ProducerAddresstBuilder.WithCountry(new AlwaysUnequalCountry());

            Assert.NotEqual(producerAddress, producerAddress2);
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

        private class ProducerAddresstBuilder
        {
            private string primaryName = "PrimaryName";
            private string secondaryName = "SecondaryName";
            private string street = "Street";
            private string town = "Town";
            private string locality = "Locality";
            private string administrativeArea = "AdministrativeArea";
            private string postCode = "PostCode";
            private Country country = new AlwaysEqualCountry();

            private ProducerAddresstBuilder()
            {
            }

            private ProducerAddress Build()
            {
                return new ProducerAddress(
                    primaryName,
                    secondaryName,
                    street,
                    town,
                    locality,
                    administrativeArea,
                    country,
                    postCode);
            }

            public static ProducerAddress NewProducerAddress
            {
                get { return new ProducerAddresstBuilder().Build(); }
            }

            public static ProducerAddress WithPrimaryName(string primaryName)
            {
                var builder = new ProducerAddresstBuilder();
                builder.primaryName = primaryName;

                return builder.Build();
            }

            public static ProducerAddress WithSecondaryName(string secondaryName)
            {
                var builder = new ProducerAddresstBuilder();
                builder.secondaryName = secondaryName;

                return builder.Build();
            }

            public static ProducerAddress WithStreet(string street)
            {
                var builder = new ProducerAddresstBuilder();
                builder.street = street;

                return builder.Build();
            }

            public static ProducerAddress WithTown(string town)
            {
                var builder = new ProducerAddresstBuilder();
                builder.town = town;

                return builder.Build();
            }

            public static ProducerAddress WithLocality(string locality)
            {
                var builder = new ProducerAddresstBuilder();
                builder.locality = locality;

                return builder.Build();
            }

            public static ProducerAddress WithAdministrativeArea(string administrativeArea)
            {
                var builder = new ProducerAddresstBuilder();
                builder.administrativeArea = administrativeArea;

                return builder.Build();
            }

            public static ProducerAddress WithPostCode(string postCode)
            {
                var builder = new ProducerAddresstBuilder();
                builder.postCode = postCode;

                return builder.Build();
            }

            public static ProducerAddress WithCountry(Country country)
            {
                var builder = new ProducerAddresstBuilder();
                builder.country = country;

                return builder.Build();
            }
        }
    }
}
