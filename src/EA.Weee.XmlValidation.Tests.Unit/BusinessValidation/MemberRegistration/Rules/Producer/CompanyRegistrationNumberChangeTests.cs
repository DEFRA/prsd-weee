namespace EA.Weee.XmlValidation.Tests.Unit.BusinessValidation.MemberRegistration.Rules.Producer
{
    using EA.Weee.XmlValidation.BusinessValidation.MemberRegistration.QuerySets;
    using EA.Weee.XmlValidation.Tests.Unit.BusinessValidation.MemberRegistration.Domain;
    using FakeItEasy;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Weee.Domain.Obligation;
    using Weee.Domain.Producer;
    using Xml.MemberRegistration;
    using XmlValidation.BusinessValidation.MemberRegistration.Rules.Producer;
    using Xunit;

    public class CompanyRegistrationNumberChangeTests
    {
        private readonly IProducerQuerySet producerQuerySet;

        public CompanyRegistrationNumberChangeTests()
        {
            producerQuerySet = A.Fake<IProducerQuerySet>();
        }

        [Fact]
        public void Evaluate_Insertion_ReturnsPass()
        {
            // Act
            var result = new CompanyRegistrationNumberChange(producerQuerySet)
                .Evaluate(new producerType() { status = statusType.I });

            // Assert
            A.CallTo(() => producerQuerySet.GetLatestProducerFromPreviousComplianceYears(A<string>._))
                .MustNotHaveHappened();

            Assert.True(result.IsValid);
        }

        [Fact]
        public void Evaluate_AmendNewProducerNotCompany_ReturnsPass()
        {
            // Act
            var result = new CompanyRegistrationNumberChange(producerQuerySet).Evaluate(new producerType() { status = statusType.A });

            // Assert
            A.CallTo(() => producerQuerySet.GetLatestProducerFromPreviousComplianceYears(A<string>._))
                .MustNotHaveHappened();

            Assert.True(result.IsValid);
        }

        [Fact]
        public void Evaluate_Amend_NoMatchingProducer_ReturnsPass()
        {
            // Arrange
            string companyNumber = "1234";

            var newProducer = new producerType()
            {
                status = statusType.A,
                registrationNo = "prn",
                producerBusiness = new producerBusinessType()
                {
                    Item = new companyType() { companyNumber = companyNumber }
                }
            };

            A.CallTo(() => producerQuerySet.GetLatestProducerFromPreviousComplianceYears("prn"))
                .Returns(null);

            // Act
            var result = new CompanyRegistrationNumberChange(producerQuerySet)
                .Evaluate(newProducer);

            // Assert
            A.CallTo(() => producerQuerySet.GetLatestProducerFromPreviousComplianceYears("prn"))
                .MustHaveHappened();

            Assert.True(result.IsValid);
        }

        [Fact]
        public void Evaluate_Amend_MatchingProducerNotACompany_ReturnsPass()
        {
            // Arrange
            string companyNumber = "1234";

            var newProducer = new producerType()
            {
                status = statusType.A,
                registrationNo = "prn",
                producerBusiness = new producerBusinessType()
                {
                    Item = new companyType() { companyNumber = companyNumber }
                }
            };

            var existingProducer = FakeProducer.Create(ObligationType.Both, "prn",
                producerBusiness: new ProducerBusiness(partnership: new Partnership("PartnerShip", A.Dummy<ProducerContact>(), new List<Partner>())));

            A.CallTo(() => producerQuerySet.GetLatestProducerFromPreviousComplianceYears("prn"))
                .Returns(existingProducer);

            // Act
            var result = new CompanyRegistrationNumberChange(producerQuerySet)
                .Evaluate(newProducer);

            // Assert
            A.CallTo(() => producerQuerySet.GetLatestProducerFromPreviousComplianceYears("prn"))
                .MustHaveHappened();

            Assert.True(result.IsValid);
        }

        [Fact]
        public void Evaluate_Amend_MatchingProducerAndCompanyNumber_ReturnsPass()
        {
            // Arrange
            string companyNumber = "1234";

            var newProducer = new producerType()
            {
                status = statusType.A,
                registrationNo = "prn",
                producerBusiness = new producerBusinessType()
                {
                    Item = new companyType() { companyNumber = companyNumber }
                }
            };

            var existingProducer = FakeProducer.Create(ObligationType.Both, "prn",
                producerBusiness: new ProducerBusiness(new Company("Company", companyNumber, null)));

            A.CallTo(() => producerQuerySet.GetLatestProducerFromPreviousComplianceYears("prn"))
                .Returns(existingProducer);

            // Act
            var result = new CompanyRegistrationNumberChange(producerQuerySet)
                .Evaluate(newProducer);

            // Assert
            A.CallTo(() => producerQuerySet.GetLatestProducerFromPreviousComplianceYears("prn"))
                .MustHaveHappened();

            Assert.True(result.IsValid);
        }

        [Fact]
        public void Evaluate_Amend_MatchingProducerDifferentCompanyNumber_ReturnsWarning()
        {
            // Arrange
            var newProducer = new producerType()
            {
                status = statusType.A,
                registrationNo = "prn",
                producerBusiness = new producerBusinessType()
                {
                    Item = new companyType() { companyNumber = "1111" }
                }
            };

            var existingProducer = FakeProducer.Create(ObligationType.Both, "prn",
                producerBusiness: new ProducerBusiness(new Company("Company", "1234", null)));

            A.CallTo(() => producerQuerySet.GetLatestProducerFromPreviousComplianceYears("prn"))
                .Returns(existingProducer);

            // Act
            var result = new CompanyRegistrationNumberChange(producerQuerySet)
                .Evaluate(newProducer);

            // Assert
            A.CallTo(() => producerQuerySet.GetLatestProducerFromPreviousComplianceYears("prn"))
                .MustHaveHappened();

            Assert.False(result.IsValid);
            Assert.Equal(Core.Shared.ErrorLevel.Warning, result.ErrorLevel);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData("000")]
        public void Evaluate_Amend_EmptyOrNullCompanyNumberForExistingCompany_ReturnsPass(string existingCompanyNumber)
        {
            // Arrange
            string companyNumber = "1234";

            var newProducer = new producerType()
            {
                status = statusType.A,
                registrationNo = "prn",
                producerBusiness = new producerBusinessType()
                {
                    Item = new companyType() { companyNumber = companyNumber }
                }
            };

            var existingProducer = FakeProducer.Create(ObligationType.Both, "prn",
                producerBusiness: new ProducerBusiness(new Company("companyName", existingCompanyNumber, null)));

            A.CallTo(() => producerQuerySet.GetLatestProducerFromPreviousComplianceYears("prn"))
                .Returns(existingProducer);

            // Act
            var result = new CompanyRegistrationNumberChange(producerQuerySet)
                .Evaluate(newProducer);

            // Assert
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData("1234", "0123")]
        [InlineData(" 123", "1234 ")]
        [InlineData("00123400", "12300")]
        [InlineData("123400", "0012300")]
        [InlineData("  0 0 1 2 3  ", "1234")]
        [InlineData("  0012 300  ", "123400")]
        [InlineData("  00123 00  ", "123400")]
        [InlineData("  1234567", " 1 2 3 4 5 6 ")]
        public void Evaluate_Amend_NonMatchingCompanyNumbersAfterFormatting_ReturnsWarning(string newCompanyNumber, string existingCompanyNumber)
        {
            // Arrange
            var newProducer = new producerType()
            {
                status = statusType.A,
                registrationNo = "prn",
                producerBusiness = new producerBusinessType()
                {
                    Item = new companyType() { companyNumber = newCompanyNumber }
                }
            };

            var existingProducer = FakeProducer.Create(ObligationType.Both, "prn",
                producerBusiness: new ProducerBusiness(new Company("Company", existingCompanyNumber, null)));

            A.CallTo(() => producerQuerySet.GetLatestProducerFromPreviousComplianceYears("prn"))
                .Returns(existingProducer);

            // Act
            var result = new CompanyRegistrationNumberChange(producerQuerySet)
                .Evaluate(newProducer);

            // Assert
            Assert.False(result.IsValid);
            Assert.Equal(Core.Shared.ErrorLevel.Warning, result.ErrorLevel);
        }

        [Theory]
        [InlineData("123", "0123")]
        [InlineData(" 123", "123 ")]
        [InlineData("0012300", "12300")]
        [InlineData("12300", "0012300")]
        [InlineData("  0 0 1 2 3  ", "123")]
        [InlineData("  0012 300  ", "12300")]
        [InlineData("  00123 00  ", "12300")]
        [InlineData("  123456", " 1 2 3 4 5 6 ")]
        public void Evaluate_Amend_MatchingCompanyNumbersAfterFormatting_ReturnsPass(string newCompanyNumber, string existingCompanyNumber)
        {
            // Arrange
            var newProducer = new producerType()
            {
                status = statusType.A,
                registrationNo = "prn",
                producerBusiness = new producerBusinessType()
                {
                    Item = new companyType() { companyNumber = newCompanyNumber }
                }
            };

            var existingProducer = FakeProducer.Create(ObligationType.Both, "prn",
                producerBusiness: new ProducerBusiness(new Company("Company", existingCompanyNumber, null)));

            A.CallTo(() => producerQuerySet.GetLatestProducerFromPreviousComplianceYears("prn"))
                .Returns(existingProducer);

            // Act
            var result = new CompanyRegistrationNumberChange(producerQuerySet)
                .Evaluate(newProducer);

            // Assert
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Evaluate_Amend_NonMatchingCompanyRegistrationNumber_WarningMessage_ContainsProducerNameAndNewCompanyRegistrationNumber()
        {
            // Arrange
            string registrationNumber = "prn";
            string existingCompanyName = "Existing company name";
            string existingCompanyNumber = "1234";
            string newCompanyNumber = "6789";

            var newProducer = new producerType()
            {
                status = statusType.A,
                registrationNo = registrationNumber,
                producerBusiness = new producerBusinessType()
                {
                    Item = new companyType() { companyName = "new company name", companyNumber = newCompanyNumber }
                }
            };

            var existingProducer = FakeProducer.Create(ObligationType.Both, registrationNumber,
                producerBusiness: new ProducerBusiness(new Company(existingCompanyName, existingCompanyNumber, null)));

            A.CallTo(() => producerQuerySet.GetLatestProducerFromPreviousComplianceYears(registrationNumber))
                .Returns(existingProducer);

            // Act
            var result = new CompanyRegistrationNumberChange(producerQuerySet)
                .Evaluate(newProducer);

            // Assert
            Assert.False(result.IsValid);
            Assert.Equal(Core.Shared.ErrorLevel.Warning, result.ErrorLevel);
            Assert.Contains(existingCompanyName, result.Message);
            Assert.Contains(registrationNumber, result.Message);
            Assert.Contains(existingCompanyNumber, result.Message);
            Assert.Contains(newCompanyNumber, result.Message);
        }

        [Fact]
        public void Evaluate_Amend_NonMatchingEmptyCompanyRegistrationNumber_WarningMessage_ContainsProducerNameAndOldCompanyRegistrationNumberOnly()
        {
            // Arrange
            string registrationNumber = "prn";
            string existingCompanyName = "Existing company name";
            string existingCompanyNumber = "1234";
            string newCompanyNumber = "000";

            var newProducer = new producerType()
            {
                status = statusType.A,
                registrationNo = registrationNumber,
                producerBusiness = new producerBusinessType()
                {
                    Item = new companyType() { companyName = "new company name", companyNumber = newCompanyNumber }
                }
            };

            var existingProducer = FakeProducer.Create(ObligationType.Both, registrationNumber,
                producerBusiness: new ProducerBusiness(new Company(existingCompanyName, existingCompanyNumber, null)));

            A.CallTo(() => producerQuerySet.GetLatestProducerFromPreviousComplianceYears(registrationNumber))
                .Returns(existingProducer);

            // Act
            var result = new CompanyRegistrationNumberChange(producerQuerySet)
                .Evaluate(newProducer);

            // Assert
            Assert.False(result.IsValid);
            Assert.Equal(Core.Shared.ErrorLevel.Warning, result.ErrorLevel);
            Assert.Contains(existingCompanyName, result.Message);
            Assert.Contains(registrationNumber, result.Message);
            Assert.Contains(existingCompanyNumber, result.Message);
            Assert.DoesNotContain(newCompanyNumber, result.Message);
        }
    }
}
