namespace EA.Weee.Domain.Tests.Unit.Producer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EA.Weee.Domain.Producer;
    using FakeItEasy;
    using Xunit;

    public class PartnerTests
    {
        [Fact]
        public void Partner_EqualsNullParameter_ReturnsFalse()
        {
            var partner = PartnerBuilder.NewPartner;

            Assert.NotEqual(partner, null);
        }

        [Fact]
        public void Partner_EqualsObjectParameter_ReturnsFalse()
        {
            var partner = PartnerBuilder.NewPartner;

            Assert.NotEqual(partner, new object());
        }

        [Fact]
        public void Partner_EqualsSameInstance_ReturnsTrue()
        {
            var partner = PartnerBuilder.NewPartner;

            Assert.Equal(partner, partner);
        }

        [Fact]
        public void Partner_EqualsPartnerSameDetails_ReturnsTrue()
        {
            var partner = PartnerBuilder.NewPartner;
            var partner2 = PartnerBuilder.NewPartner;

            Assert.Equal(partner, partner2);
        }

        [Fact]
        public void Partner_EqualsPartnerDifferentName_ReturnsFalse()
        {
            var partner = PartnerBuilder.NewPartner;
            var partner2 = PartnerBuilder.WithName("test name");

            Assert.NotEqual(partner, partner2);
        }

        [Fact]
        public void Partner_CompareToNull_ReturnsOne()
        {
            var partner = PartnerBuilder.NewPartner;

            Assert.Equal(1, partner.CompareTo(null));
        }

        [Fact]
        public void Partner_CompareToPartnerSameName_ReturnsZero()
        {
            var partner = PartnerBuilder.NewPartner;
            var partner2 = PartnerBuilder.NewPartner;

            Assert.Equal(0, partner.CompareTo(partner2));
        }

        [Fact]
        public void Partner_CompareToPartnerGreaterName_ReturnsMinusOne()
        {
            var partner = PartnerBuilder.NewPartner;
            var partner2 = PartnerBuilder.WithName("zzz");

            Assert.Equal(-1, partner.CompareTo(partner2));
        }

        [Fact]
        public void Partner_CompareToPartnerLesserName_ReturnsOne()
        {
            var partner = PartnerBuilder.NewPartner;
            var partner2 = PartnerBuilder.WithName("aaa");

            Assert.Equal(1, partner.CompareTo(partner2));
        }

        private class PartnerBuilder
        {
            private string name = "name";

            private PartnerBuilder()
            {
            }

            private Partner Build()
            {
                return new Partner(name);
            }

            public static Partner NewPartner
            {
                get { return new PartnerBuilder().Build(); }
            }

            public static Partner WithName(string name)
            {
                var builder = new PartnerBuilder();
                builder.name = name;

                return builder.Build();
            }
        }
    }
}
