namespace EA.Weee.XmlValidation.Tests.Unit.BusinessValidation.MemberRegistration.Rules.Producer
{
    using System.Collections.Generic;
    using Domain;
    using FakeItEasy;
    using Weee.Domain;
    using Weee.Domain.Obligation;
    using Weee.Domain.Producer;
    using Xml.MemberRegistration;
    using XmlValidation.BusinessValidation.MemberRegistration.QuerySets;
    using XmlValidation.BusinessValidation.MemberRegistration.Rules.Producer;
    using Xunit;

    public class CompanyAlreadyRegisteredTests
    {
        private readonly IProducerQuerySet producerQuerySet;
        private readonly IMigratedProducerQuerySet migratedProducerQuerySet;

        public CompanyAlreadyRegisteredTests()
        {
            producerQuerySet = A.Fake<IProducerQuerySet>();
            migratedProducerQuerySet = A.Fake<IMigratedProducerQuerySet>();
        }

        [Fact]
        public void Evaluate_Amendment_ReturnsPass()
        {
            var result = new CompanyAlreadyRegistered(producerQuerySet, migratedProducerQuerySet).Evaluate(new producerType() { status = statusType.A });
            
            A.CallTo(() => producerQuerySet.GetLatestCompanyProducers()).MustNotHaveHappened();
            A.CallTo(() => migratedProducerQuerySet.GetAllMigratedProducers()).MustNotHaveHappened();

            Assert.True(result.IsValid);
        }

        [Fact]
        public void Evaluate_InsertNotCompanyProducer_ReturnsPass()
        {
            var result = new CompanyAlreadyRegistered(producerQuerySet, migratedProducerQuerySet).Evaluate(new producerType() { status = statusType.I });

            A.CallTo(() => producerQuerySet.GetLatestCompanyProducers()).MustNotHaveHappened();
            A.CallTo(() => migratedProducerQuerySet.GetAllMigratedProducers()).MustNotHaveHappened();

            Assert.True(result.IsValid);
        }

        [Fact]
        public void Evaluate_Insert_MatchingCompanyRegistrationNumber_ReturnsError()
        {
            string companyNumber = "1234";

            var newProducer = new producerType()
            {
                status = statusType.I,
                producerBusiness = new producerBusinessType()
                {
                    Item = new companyType() { companyNumber = companyNumber }
                }
            };

            var existingProducer = FakeProducer.Create(ObligationType.Both, "prn",
                producerBusiness: new ProducerBusiness(new Company("Company", companyNumber, null)));

            A.CallTo(() => producerQuerySet.GetLatestCompanyProducers()).Returns(new List<ProducerSubmission>() { existingProducer });

            var result = new CompanyAlreadyRegistered(producerQuerySet, migratedProducerQuerySet).Evaluate(newProducer);

            Assert.False(result.IsValid);
            Assert.Equal(Core.Shared.ErrorLevel.Error, result.ErrorLevel);
        }

        [Fact]
        public void Evaluate_Insert_MatchingMigratedProducerCompanyRegistrationNumber_ReturnsError()
        {
            string companyNumber = "1234";

            var newProducer = new producerType()
            {
                status = statusType.I,
                producerBusiness = new producerBusinessType()
                {
                    Item = new companyType() { companyNumber = companyNumber }
                }
            };

            A.CallTo(() => producerQuerySet.GetLatestCompanyProducers()).Returns(new List<ProducerSubmission>());

            var migratedProducer = A.Fake<MigratedProducer>();
            A.CallTo(() => migratedProducer.CompanyNumber).Returns(companyNumber);

            A.CallTo(() => migratedProducerQuerySet.GetAllMigratedProducers()).Returns(new List<MigratedProducer>() { migratedProducer });

            var result = new CompanyAlreadyRegistered(producerQuerySet, migratedProducerQuerySet).Evaluate(newProducer);

            Assert.False(result.IsValid);
            Assert.Equal(Core.Shared.ErrorLevel.Error, result.ErrorLevel);
        }

        [Fact]
        public void Evaluate_Insert_MatchingCompanyRegistrationNumber_ErrorMessage_ContainsProducerNameAndCompanyRegistrationNumber()
        {
            string companyName = "Test company name";
            string companyNumber = "1234";

            var newProducer = new producerType()
            {
                status = statusType.I,
                producerBusiness = new producerBusinessType()
                {
                    Item = new companyType() { companyName = companyName, companyNumber = companyNumber }
                }
            };

            var existingProducer = FakeProducer.Create(ObligationType.Both, "prn",
                producerBusiness: new ProducerBusiness(new Company(companyName, companyNumber, null)));

            A.CallTo(() => producerQuerySet.GetLatestCompanyProducers()).Returns(new List<ProducerSubmission>() { existingProducer });

            var result = new CompanyAlreadyRegistered(producerQuerySet, migratedProducerQuerySet).Evaluate(newProducer);

            Assert.False(result.IsValid);
            Assert.Equal(Core.Shared.ErrorLevel.Error, result.ErrorLevel);
            Assert.Contains(companyName, result.Message);
            Assert.Contains(companyNumber, result.Message);
        }

        [Fact]
        public void Evaluate_Insert_MatchingMigratedProducerCompanyRegistrationNumber_ErrorMessage_ContainsProducerNameAndCompanyRegistrationNumber()
        {
            string companyName = "Test company name";
            string companyNumber = "1234";

            var newProducer = new producerType()
            {
                status = statusType.I,
                producerBusiness = new producerBusinessType()
                {
                    Item = new companyType() { companyName = companyName, companyNumber = companyNumber }
                }
            };

            A.CallTo(() => producerQuerySet.GetLatestCompanyProducers()).Returns(new List<ProducerSubmission>());

            var migratedProducer = A.Fake<MigratedProducer>();
            A.CallTo(() => migratedProducer.CompanyNumber).Returns(companyNumber);

            A.CallTo(() => migratedProducerQuerySet.GetAllMigratedProducers()).Returns(new List<MigratedProducer>() { migratedProducer });

            var result = new CompanyAlreadyRegistered(producerQuerySet, migratedProducerQuerySet).Evaluate(newProducer);

            Assert.False(result.IsValid);
            Assert.Equal(Core.Shared.ErrorLevel.Error, result.ErrorLevel);
            Assert.Contains(companyName, result.Message);
            Assert.Contains(companyNumber, result.Message);
        }

        [Fact]
        public void Evaluate_Insert_MatchingCompanyRegistrationNumber_DoesNotPerformMigratedProducerCompanyRegistrationNumberCheck()
        {
            string companyName = "Test company name";
            string companyNumber = "1234";

            var newProducer = new producerType()
            {
                status = statusType.I,
                producerBusiness = new producerBusinessType()
                {
                    Item = new companyType() { companyName = companyName, companyNumber = companyNumber }
                }
            };

            var existingProducer = FakeProducer.Create(ObligationType.Both, "prn",
                producerBusiness: new ProducerBusiness(new Company(companyName, companyNumber, null)));

            A.CallTo(() => producerQuerySet.GetLatestCompanyProducers()).Returns(new List<ProducerSubmission>() { existingProducer });

            new CompanyAlreadyRegistered(producerQuerySet, migratedProducerQuerySet).Evaluate(newProducer);

            A.CallTo(() => producerQuerySet.GetLatestCompanyProducers()).MustHaveHappened();
            A.CallTo(() => migratedProducerQuerySet.GetAllMigratedProducers()).MustNotHaveHappened();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData("000")]
        public void Evaluate_Insert_EmptyOrNullCompanyNumberForNewCompany_DoesNotCompareRegistrationNumbers_ReturnsPass(string newCompanyNumber)
        {
            var newProducer = new producerType()
            {
                status = statusType.I,
                producerBusiness = new producerBusinessType()
                {
                    Item = new companyType() { companyNumber = newCompanyNumber }
                }
            };

            var result = new CompanyAlreadyRegistered(producerQuerySet, migratedProducerQuerySet).Evaluate(newProducer);

            A.CallTo(() => producerQuerySet.GetLatestCompanyProducers()).MustNotHaveHappened();
            A.CallTo(() => migratedProducerQuerySet.GetAllMigratedProducers()).MustNotHaveHappened();

            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData("000")]
        public void Evaluate_Insert_EmptyOrNullCompanyNumberForExistingCompany_ReturnsPass(string existingCompanyNumber)
        {
            string companyNumber = "1234";

            var newProducer = new producerType()
            {
                status = statusType.I,
                producerBusiness = new producerBusinessType()
                {
                    Item = new companyType() { companyNumber = companyNumber }
                }
            };

            var existingProducer = FakeProducer.Create(ObligationType.Both, "prn",
                producerBusiness: new ProducerBusiness(new Company("companyName", existingCompanyNumber, null)));

            A.CallTo(() => producerQuerySet.GetLatestCompanyProducers()).Returns(new List<ProducerSubmission>() { existingProducer });

            var result = new CompanyAlreadyRegistered(producerQuerySet, migratedProducerQuerySet).Evaluate(newProducer);
            
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData("000")]
        public void Evaluate_Insert_EmptyOrNullCompanyNumberForMigratedProducerCompany_ReturnsPass(string migratedProducerCompanyNumber)
        {
            string companyNumber = "1234";

            var newProducer = new producerType()
            {
                status = statusType.I,
                producerBusiness = new producerBusinessType()
                {
                    Item = new companyType() { companyNumber = companyNumber }
                }
            };

            A.CallTo(() => producerQuerySet.GetLatestCompanyProducers()).Returns(new List<ProducerSubmission>());

            var migratedProducer = A.Fake<MigratedProducer>();
            A.CallTo(() => migratedProducer.CompanyNumber).Returns(migratedProducerCompanyNumber);

            A.CallTo(() => migratedProducerQuerySet.GetAllMigratedProducers()).Returns(new List<MigratedProducer>() { migratedProducer });

            var result = new CompanyAlreadyRegistered(producerQuerySet, migratedProducerQuerySet).Evaluate(newProducer);

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
        public void Evaluate_Insert_NonMatchingCompanyNumbersAfterFormatting_ReturnsPass(string newCompanyNumber, string existingCompanyNumber)
        {
            var newProducer = new producerType()
            {
                status = statusType.I,
                producerBusiness = new producerBusinessType()
                {
                    Item = new companyType() { companyNumber = newCompanyNumber }
                }
            };

            var existingProducer = FakeProducer.Create(ObligationType.Both, "prn",
                producerBusiness: new ProducerBusiness(new Company("Company", existingCompanyNumber, null)));

            A.CallTo(() => producerQuerySet.GetLatestCompanyProducers()).Returns(new List<ProducerSubmission>() { existingProducer });

            var result = new CompanyAlreadyRegistered(producerQuerySet, migratedProducerQuerySet).Evaluate(newProducer);

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
        public void Evaluate_Insert_NonMatchingMigratedProducerCompanyNumbersAfterFormatting_ReturnsPass(string newCompanyNumber, string migratedProducerCompanyNumber)
        {
            var newProducer = new producerType()
            {
                status = statusType.I,
                producerBusiness = new producerBusinessType()
                {
                    Item = new companyType() { companyNumber = newCompanyNumber }
                }
            };

            A.CallTo(() => producerQuerySet.GetLatestCompanyProducers()).Returns(new List<ProducerSubmission>());

            var migratedProducer = A.Fake<MigratedProducer>();
            A.CallTo(() => migratedProducer.CompanyNumber).Returns(migratedProducerCompanyNumber);

            A.CallTo(() => migratedProducerQuerySet.GetAllMigratedProducers()).Returns(new List<MigratedProducer>() { migratedProducer });

            var result = new CompanyAlreadyRegistered(producerQuerySet, migratedProducerQuerySet).Evaluate(newProducer);

            Assert.True(result.IsValid);
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
        public void Evaluate_Insert_MatchingCompanyNumbersAfterFormatting_ReturnsError(string newCompanyNumber, string existingCompanyNumber)
        {
            var newProducer = new producerType()
            {
                status = statusType.I,
                producerBusiness = new producerBusinessType()
                {
                    Item = new companyType() { companyNumber = newCompanyNumber }
                }
            };

            var existingProducer = FakeProducer.Create(ObligationType.Both, "prn",
                producerBusiness: new ProducerBusiness(new Company("Company", existingCompanyNumber, null)));

            A.CallTo(() => producerQuerySet.GetLatestCompanyProducers()).Returns(new List<ProducerSubmission>() { existingProducer });

            var result = new CompanyAlreadyRegistered(producerQuerySet, migratedProducerQuerySet).Evaluate(newProducer);

            Assert.False(result.IsValid);
            Assert.Equal(Core.Shared.ErrorLevel.Error, result.ErrorLevel);
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
        public void Evaluate_Insert_MatchingMigratedProducerCompanyNumbersAfterFormatting_ReturnsError(string newCompanyNumber, string migratedProducerCompanyNumber)
        {
            var newProducer = new producerType()
            {
                status = statusType.I,
                producerBusiness = new producerBusinessType()
                {
                    Item = new companyType() { companyNumber = newCompanyNumber }
                }
            };

            A.CallTo(() => producerQuerySet.GetLatestCompanyProducers()).Returns(new List<ProducerSubmission>());

            var migratedProducer = A.Fake<MigratedProducer>();
            A.CallTo(() => migratedProducer.CompanyNumber).Returns(migratedProducerCompanyNumber);

            A.CallTo(() => migratedProducerQuerySet.GetAllMigratedProducers()).Returns(new List<MigratedProducer>() { migratedProducer });

            var result = new CompanyAlreadyRegistered(producerQuerySet, migratedProducerQuerySet).Evaluate(newProducer);

            Assert.False(result.IsValid);
            Assert.Equal(Core.Shared.ErrorLevel.Error, result.ErrorLevel);
        }
    }
}