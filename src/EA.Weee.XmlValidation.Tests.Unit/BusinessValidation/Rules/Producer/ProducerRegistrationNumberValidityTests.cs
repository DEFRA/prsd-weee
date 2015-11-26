namespace EA.Weee.XmlValidation.Tests.Unit.BusinessValidation.Rules.Producer
{
    using Core.Shared;
    using FakeItEasy;
    using System.Collections.Generic;
    using Xml.MemberRegistration;
    using XmlValidation.BusinessValidation.QuerySets;
    using XmlValidation.BusinessValidation.Rules.Producer;
    using Xunit;

    public class ProducerRegistrationNumberValidityTests
    {
        private readonly IProducerQuerySet producerQuerySet;
        private readonly IMigratedProducerQuerySet migratedProducerQuerySet;

        public ProducerRegistrationNumberValidityTests()
        {
            producerQuerySet = A.Fake<IProducerQuerySet>();
            migratedProducerQuerySet = A.Fake<IMigratedProducerQuerySet>();

            // By default, no prns returned by query set
            A.CallTo(() => migratedProducerQuerySet.GetAllRegistrationNumbers())
                .Returns(new List<string>());

            // By default, no prns returned by query set
            A.CallTo(() => producerQuerySet.GetAllRegistrationNumbers())
                .Returns(new List<string>());
        }

        [Fact]
        public void ProducerHasPrnNumberThatDoesExist_ValidationSucceeds()
        {
            const string prn = "ABC12345";

            A.CallTo(() => producerQuerySet.GetAllRegistrationNumbers())
                .Returns(new List<string> { prn });

            var producer = new producerType()
            {
                registrationNo = prn,
                status = statusType.A
            };

            var result = Rule().Evaluate(producer);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void ProducerHasMigratedPrnNumberThatDoesExist_ValidationSucceeds()
        {
            const string prn = "ABC12345";

            A.CallTo(() => migratedProducerQuerySet.GetAllRegistrationNumbers())
                .Returns(new List<string> { prn });

            var producer = new producerType()
            {
                registrationNo = prn,
                status = statusType.A
            };

            var result = Rule().Evaluate(producer);

            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData("ABC12345", statusType.I)]
        [InlineData(null, statusType.I)]
        [InlineData("", statusType.I)]
        [InlineData(null, statusType.A)]
        [InlineData("", statusType.A)]
        public void InsertProducer_Or_ProducerHasNoRegistrationNumber_ValidationSucceeds(string prn, statusType status)
        {
            var producer = new producerType()
            {
                registrationNo = prn,
                status = status
            };

            var result = Rule().Evaluate(producer);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void ProducerHasPrnNumberThatDoesNotExist_IsInvalid_IncludesPrnInMessage_AndErrorLevelIsError()
        {
            const string prn = "ABC12345";
            var producer = new producerType()
            {
                registrationNo = prn,
                status = statusType.A
            };

            var result = Rule().Evaluate(producer);

            Assert.False(result.IsValid);

            Assert.Contains(prn, result.Message);
            Assert.Equal(ErrorLevel.Error, result.ErrorLevel);
        }

        [Fact]
        public void ProducerHasPrnNumberThatDoesNotExistAndProducerIsPartnership_ReturnsProducerNameInErrorMessage()
        {
            const string prn = "ABC12345";
            const string partnershipName = "Partnership Name";

            var producer = new producerType()
            {
                registrationNo = prn,
                status = statusType.A,
                producerBusiness = new producerBusinessType
                {
                    Item = new partnershipType
                    {
                        partnershipName = partnershipName
                    }
                },
            };

            var result = Rule().Evaluate(producer);

            Assert.Contains(producer.GetProducerName(), result.Message);
        }

        [Fact]
        public void ProducerHasPrnNumberThatDoesNotExistAndProducerIsCompany_ReturnsProducerNameInErrorMessage()
        {
            const string prn = "ABC12345";
            const string companyName = "Company Name";
            var producer = new producerType
            {
                registrationNo = prn,
                status = statusType.A,
                producerBusiness = new producerBusinessType
                {
                    Item = new companyType
                    {
                        companyName = companyName
                    }
                },
            };

            var result = Rule().Evaluate(producer);

            Assert.Contains(producer.GetProducerName(), result.Message);
        }

        private ProducerRegistrationNumberValidity Rule()
        {
            return new ProducerRegistrationNumberValidity(producerQuerySet, migratedProducerQuerySet);
        }
    }
}
