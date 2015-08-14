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

    public class CompanyTests
    {
        [Fact]
        public void Company_EqualsNullParameter_ReturnsFalse()
        {
            var company = CompanyBuilder.NewCompany;

            Assert.NotEqual(company, null);
        }

        [Fact]
        public void Company_EqualsObjectParameter_ReturnsFalse()
        {
            var company = CompanyBuilder.NewCompany;

            Assert.NotEqual(company, new object());
        }

        [Fact]
        public void Company_EqualsSameInstance_ReturnsTrue()
        {
            var company = CompanyBuilder.NewCompany;

            Assert.Equal(company, company);
        }

        [Fact]
        public void Company_EqualsCompanySameDetails_ReturnsTrue()
        {
            var company = CompanyBuilder.NewCompany;
            var company2 = CompanyBuilder.NewCompany;

            Assert.Equal(company, company2);
        }

        [Fact]
        public void Company_EqualsCompanyDifferentName_ReturnsFalse()
        {
            var company = CompanyBuilder.NewCompany;
            var company2 = CompanyBuilder.WithName("test name");

            Assert.NotEqual(company, company2);
        }

        [Fact]
        public void Company_EqualsCompanyDifferentRegistrationNumber_ReturnsFalse()
        {
            var company = CompanyBuilder.NewCompany;
            var company2 = CompanyBuilder.WithRegistrationNumber("test registration number");

            Assert.NotEqual(company, company2);
        }

        [Fact]
        public void Company_EqualsCompanyDifferentProducerContact_ReturnsFalse()
        {
            var company = CompanyBuilder.WithProducerContact(new AlwaysUnequalProducerContact());
            var company2 = CompanyBuilder.WithProducerContact(new AlwaysUnequalProducerContact());

            Assert.NotEqual(company, company2);
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

        private class CompanyBuilder
        {
            private string name = "name";
            private string registrationNumber = "registrationNumber";
            private ProducerContact contact = new AlwaysEqualProducerContact();

            private Company Build()
            {
                return new Company(name, registrationNumber, contact);
            }

            public static Company NewCompany
            {
                get { return new CompanyBuilder().Build(); }
            }

            public static Company WithName(string name)
            {
                var builder = new CompanyBuilder();
                builder.name = name;

                return builder.Build();
            }

            public static Company WithRegistrationNumber(string registrationNumber)
            {
                var builder = new CompanyBuilder();
                builder.registrationNumber = registrationNumber;

                return builder.Build();
            }

            public static Company WithProducerContact(ProducerContact contact)
            {
                var builder = new CompanyBuilder();
                builder.contact = contact;

                return builder.Build();
            }
        }
    }
}
