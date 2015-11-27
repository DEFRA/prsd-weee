namespace EA.Weee.Domain.Tests.Unit.Producer
{
    using System.Collections.Generic;
    using EA.Weee.Domain.Producer;
    using Xunit;

    public class PartnershipTests
    {
        [Fact]
        public void Partnership_EqualsNullParameter_ReturnsFalse()
        {
            var partnership = PartnershipBuilder.NewPartnership;

            Assert.NotEqual(partnership, null);
        }

        [Fact]
        public void Partnership_EqualsObjectParameter_ReturnsFalse()
        {
            var partnership = PartnershipBuilder.NewPartnership;

            Assert.NotEqual(partnership, new object());
        }

        [Fact]
        public void Partnership_EqualsSameInstance_ReturnsTrue()
        {
            var partnership = PartnershipBuilder.NewPartnership;

            Assert.Equal(partnership, partnership);
        }

        [Fact]
        public void Partnership_EqualsPartnershipSameDetails_ReturnsTrue()
        {
            var partnership = PartnershipBuilder.NewPartnership;
            var partnership2 = PartnershipBuilder.NewPartnership;

            Assert.Equal(partnership, partnership2);
        }

        [Fact]
        public void Partnership_EqualsPartnershipDifferentName_ReturnsFalse()
        {
            var partnership = PartnershipBuilder.NewPartnership;
            var partnership2 = PartnershipBuilder.WithName("name test");

            Assert.NotEqual(partnership, partnership2);
        }

        [Fact]
        public void Partnership_EqualsPartnershipDifferentPrincipalPlaceOfBusiness_ReturnsFalse()
        {
            var partnership = PartnershipBuilder.WithPrincipalPlaceOfBusiness(new AlwaysUnequalProducerContact());
            var partnership2 = PartnershipBuilder.WithPrincipalPlaceOfBusiness(new AlwaysUnequalProducerContact());

            Assert.NotEqual(partnership, partnership2);
        }

        [Fact]
        public void Partnership_EqualsPartnershipDifferentPartnersList_ReturnsFalse()
        {
            var partnerList1 = new List<Partner> { new Partner("Partner A"), new Partner("Partner B") };
            var partnerList2 = new List<Partner> { new Partner("Partner C"), new Partner("Partner A") };

            var partnership = PartnershipBuilder.WithPartnersList(partnerList1);
            var partnership2 = PartnershipBuilder.WithPartnersList(partnerList2);

            Assert.NotEqual(partnership, partnership2);
        }

        private class AlwaysEqualProducerContact : ProducerContact
        {
            public override bool Equals(ProducerContact other)
            {
                return true;
            }
        }

        private class AlwaysUnequalProducerContact : ProducerContact
        {
            public override bool Equals(ProducerContact other)
            {
                return false;
            }
        }

        private class PartnershipBuilder
        {
            private string name = "name";
            private ProducerContact principalPlaceOfBusiness = new AlwaysEqualProducerContact();
            private List<Partner> partners;

            private PartnershipBuilder()
            {
                partners = new List<Partner>();
                partners.Add(new Partner("Partner1"));
                partners.Add(new Partner("Partner2"));
            }

            private Partnership Build()
            {
                return new Partnership(name, principalPlaceOfBusiness, partners);
            }

            public static Partnership NewPartnership
            {
                get { return new PartnershipBuilder().Build(); }
            }

            public static Partnership WithName(string name)
            {
                var builder = new PartnershipBuilder();
                builder.name = name;

                return builder.Build();
            }

            public static Partnership WithPrincipalPlaceOfBusiness(ProducerContact contact)
            {
                var builder = new PartnershipBuilder();
                builder.principalPlaceOfBusiness = contact;

                return builder.Build();
            }

            public static Partnership WithPartnersList(List<Partner> partners)
            {
                var builder = new PartnershipBuilder();
                builder.partners = partners;

                return builder.Build();
            }
        }
    }
}
