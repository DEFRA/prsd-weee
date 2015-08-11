namespace EA.Weee.Domain.Tests.Unit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class CountryTests
    {
        [Fact]
        public void Country_EqualsNullParameter_ReturnsFalse()
        {
            var country = NewCountry();

            Assert.NotEqual(country, null);
        }

        [Fact]
        public void Country_EqualsObjectParameter_ReturnsFalse()
        {
            var country = NewCountry();

            Assert.NotEqual(country, new object());
        }

        [Fact]
        public void Country_EqualsSameInstance_ReturnsTrue()
        {
            var country = NewCountry();

            Assert.Equal(country, country);
        }

        [Fact]
        public void Country_EqualsCountrySameName_ReturnsTrue()
        {
            var country = NewCountry();
            var country2 = NewCountry();

            Assert.Equal(country, country2);
        }

        [Fact]
        public void Country_EqualsCountryDifferentName_ReturnsFalse()
        {
            var country = NewCountry();
            var country2 = new Country(Guid.Empty, "Different country name");

            Assert.NotEqual(country, country2);
        }

        private Country NewCountry()
        {
            return new Country(Guid.Empty, "TestCountry");
        }
    }
}
