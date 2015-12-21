namespace EA.Weee.Domain.Tests.Unit.Producer
{
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

        [Fact]
        public void BrandName_CompareToNull_ReturnsOne()
        {
            var brandName = BrandNameBuilder.NewBrandName;

            Assert.Equal(1, brandName.CompareTo(null));
        }

        [Fact]
        public void BrandName_CompareToBrandNameSameName_ReturnsZero()
        {
            var brandName = BrandNameBuilder.NewBrandName;
            var brandName2 = BrandNameBuilder.NewBrandName;

            Assert.Equal(0, brandName.CompareTo(brandName2));
        }

        [Fact]
        public void BrandName_CompareToBrandNameGreaterName_ReturnsMinusOne()
        {
            var brandName = BrandNameBuilder.NewBrandName;
            var brandName2 = BrandNameBuilder.WithName("zzz");

            Assert.Equal(-1, brandName.CompareTo(brandName2));
        }

        [Fact]
        public void BrandName_CompareToBrandNameLesserName_ReturnsOne()
        {
            var brandName = BrandNameBuilder.NewBrandName;
            var brandName2 = BrandNameBuilder.WithName("aaa");

            Assert.Equal(1, brandName.CompareTo(brandName2));
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
