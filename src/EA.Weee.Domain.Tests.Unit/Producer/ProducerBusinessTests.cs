namespace EA.Weee.Domain.Tests.Unit.Producer
{
    using EA.Weee.Domain.Producer;
    using Xunit;

    public class ProducerBusinessTests
    {
        [Fact]
        public void ProducerBusiness_EqualsNullParameter_ReturnsFalse()
        {
            var producerBusiness = ProducerBusinessBuilder.NewProducerBusiness;

            Assert.NotEqual(producerBusiness, null);
        }

        [Fact]
        public void ProducerBusiness_EqualsObjectParameter_ReturnsFalse()
        {
            var producerBusiness = ProducerBusinessBuilder.NewProducerBusiness;

            Assert.NotEqual(producerBusiness, new object());
        }

        [Fact]
        public void ProducerBusiness_EqualsSameInstance_ReturnsTrue()
        {
            var producerBusiness = ProducerBusinessBuilder.NewProducerBusiness;

            Assert.Equal(producerBusiness, producerBusiness);
        }

        [Fact]
        public void ProducerBusiness_EqualsProducerBusinessSameDetails_ReturnsTrue()
        {
            var producerBusiness = ProducerBusinessBuilder.NewProducerBusiness;
            var producerBusiness2 = ProducerBusinessBuilder.NewProducerBusiness;

            Assert.Equal(producerBusiness, producerBusiness2);
        }

        [Fact]
        public void ProducerBusiness_EqualsProducerBusinessDifferentCompanyDetails_ReturnsFalse()
        {
            var producerBusiness = ProducerBusinessBuilder.WithCompanyDetails(new AlwaysUnequalCompany());
            var producerBusiness2 = ProducerBusinessBuilder.WithCompanyDetails(new AlwaysUnequalCompany());

            Assert.NotEqual(producerBusiness, producerBusiness2);
        }

        [Fact]
        public void ProducerBusiness_EqualsProducerBusinessDifferentPartnership_ReturnsFalse()
        {
            var producerBusiness = ProducerBusinessBuilder.WithPartnership(new AlwaysUnequalPartnership());
            var producerBusiness2 = ProducerBusinessBuilder.WithPartnership(new AlwaysUnequalPartnership());

            Assert.NotEqual(producerBusiness, producerBusiness2);
        }

        [Fact]
        public void ProducerBusiness_EqualsProducerBusinessDifferentCorrespondentForNoticesContact_ReturnsFalse()
        {
            var producerBusiness = ProducerBusinessBuilder.WithCorrespondentForNoticesContact(new AlwaysUnequalProducerContact());
            var producerBusiness2 = ProducerBusinessBuilder.WithCorrespondentForNoticesContact(new AlwaysUnequalProducerContact());

            Assert.NotEqual(producerBusiness, producerBusiness2);
        }

        private class AlwaysEqualCompany : Company
        {
            public override bool Equals(Company other)
            {
                return true;
            }
        }

        private class AlwaysUnequalCompany : Company
        {
            public override bool Equals(Company other)
            {
                return false;
            }
        }

        private class AlwaysEqualPartnership : Partnership
        {
            public override bool Equals(Partnership other)
            {
                return true;
            }
        }

        private class AlwaysUnequalPartnership : Partnership
        {
            public override bool Equals(Partnership other)
            {
                return false;
            }
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

        private class ProducerBusinessBuilder
        {
            private Company companyDetails = new AlwaysEqualCompany();
            private Partnership partnership = new AlwaysEqualPartnership();
            private ProducerContact correspondentForNoticesContact = new AlwaysEqualProducerContact();

            private ProducerBusiness Build()
            {
                return new ProducerBusiness(companyDetails, partnership, correspondentForNoticesContact);
            }

            public static ProducerBusiness NewProducerBusiness
            {
                get { return new ProducerBusinessBuilder().Build(); }
            }

            public static ProducerBusiness WithCompanyDetails(Company company)
            {
                var builder = new ProducerBusinessBuilder();
                builder.companyDetails = company;

                return builder.Build();
            }

            public static ProducerBusiness WithPartnership(Partnership partnership)
            {
                var builder = new ProducerBusinessBuilder();
                builder.partnership = partnership;

                return builder.Build();
            }

            public static ProducerBusiness WithCorrespondentForNoticesContact(ProducerContact contact)
            {
                var builder = new ProducerBusinessBuilder();
                builder.correspondentForNoticesContact = contact;

                return builder.Build();
            }
        }
    }
}
