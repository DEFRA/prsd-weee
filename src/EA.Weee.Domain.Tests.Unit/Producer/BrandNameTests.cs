namespace EA.Weee.Domain.Tests.Unit.Producer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EA.Weee.Domain.Producer;
    using Xunit;

    public class BrandNameTests
    {
        [Fact]
        public void BrandName_EqualsNullParameter_ReturnsFalse()
        {
            var brandName = BrandNameBuilder.NewBrandName;

            Assert.NotEqual(brandName, null);
        }

        [Fact]
        public void BrandName_EqualsObjectParameter_ReturnsFalse()
        {
            var brandName = BrandNameBuilder.NewBrandName;

            Assert.NotEqual(brandName, new object());
        }

        [Fact]
        public void BrandName_EqualsSameInstance_ReturnsTrue()
        {
            var brandName = BrandNameBuilder.NewBrandName;

            Assert.Equal(brandName, brandName);
        }

        [Fact]
        public void BrandName_EqualsBrandNameSameDetails_ReturnsTrue()
        {
            var brandName = BrandNameBuilder.NewBrandName;
            var brandName2 = BrandNameBuilder.NewBrandName;

            Assert.Equal(brandName, brandName2);
        }

        [Fact]
        public void BrandName_EqualsBrandNameDifferentName_ReturnsFalse()
        {
            var brandName = BrandNameBuilder.NewBrandName;
            var brandName2 = BrandNameBuilder.WithName("test name");

            Assert.NotEqual(brandName, brandName2);
        }

        private class BrandNameBuilder
        {
            private string name = "name";

            private BrandNameBuilder()
            {
            }

            private BrandName Build()
            {
                return new BrandName(name);
            }

            public static BrandName NewBrandName
            {
                get { return new BrandNameBuilder().Build(); }
            }

            public static BrandName WithName(string name)
            {
                var builder = new BrandNameBuilder();
                builder.name = name;

                return builder.Build();
            }
        }
    }
}
