namespace EA.Weee.XmlValidation.Tests.Unit.BusinessValidation.MemberRegistration.Rules.Producer
{
    using System;
    using Domain;
    using FakeItEasy;
    using Weee.Domain;
    using Weee.Domain.Producer;
    using Xml.MemberRegistration;
    using XmlValidation.BusinessValidation.MemberRegistration.QuerySets;
    using XmlValidation.BusinessValidation.MemberRegistration.Rules.Producer;
    using Xunit;
    using schemeType = Xml.MemberRegistration.schemeType;

    public class ProducerNameChangeTests
    {
        private readonly IProducerQuerySet producerQuerySet;
        private readonly IMigratedProducerQuerySet migratedProducerQuerySet;

        public ProducerNameChangeTests()
        {
            producerQuerySet = A.Fake<IProducerQuerySet>();
            migratedProducerQuerySet = A.Fake<IMigratedProducerQuerySet>();

            // By default, nulls returned from queries
            A.CallTo(() => producerQuerySet.GetLatestProducerForComplianceYearAndScheme(A<string>._, A<string>._, A<Guid>._))
                .Returns(null);
            A.CallTo(() => producerQuerySet.GetLatestProducerFromPreviousComplianceYears(A<string>._))
                .Returns(null);
            A.CallTo(() => migratedProducerQuerySet.GetMigratedProducer(A<string>._))
                .Returns(null);
            A.CallTo(() => producerQuerySet.GetProducerForOtherSchemeAndObligationType(A<string>._, A<string>._, A<Guid>._, A<int>._))
                .Returns(null);
        }

        [Fact]
        public void Insert_AndProducerExistsWithMatchingPrnInComplianceYearAndScheme_ReturnsPass()
        {
            A.CallTo(() => producerQuerySet.GetLatestProducerForComplianceYearAndScheme(A<string>._, A<string>._, A<Guid>._))
                .Returns(FakeProducer.Create(ObligationType.Both, "ABC12345"));

            var result = Rule().Evaluate(new schemeType(), new producerType
            {
                status = statusType.I,
                producerBusiness = new producerBusinessType
                {
                    Item = new partnershipType
                    {
                        partnershipName = "New Producer Name"
                    }
                }
            }, A<Guid>._);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void Amendment_AndProducerExistsWithMatchingPrnInComplianceYearAndScheme_ReturnsFailAsWarning()
        {
            A.CallTo(() => producerQuerySet.GetLatestProducerForComplianceYearAndScheme(A<string>._, A<string>._, A<Guid>._))
                .Returns(FakeProducer.Create(ObligationType.Both, "ABC12345"));

            var result = Rule().Evaluate(new schemeType(), new producerType
            {
                status = statusType.A,
                producerBusiness = new producerBusinessType
                {
                    Item = new partnershipType
                    {
                        partnershipName = "New Producer Name"
                    }
                }
            }, A<Guid>._);

            Assert.False(result.IsValid);
            Assert.Equal(Core.Shared.ErrorLevel.Warning, result.ErrorLevel);
        }

        [Fact]
        public void Amendment_AndProducerExistsWithMatchingPrnInPreviousComplianceYear_ReturnsFailAsWarning_AndIncludesProducerNamesInErrorMessage()
        {
            A.CallTo(() => producerQuerySet.GetLatestProducerFromPreviousComplianceYears(A<string>._))
                .Returns(FakeProducer.Create(ObligationType.Both, "ABC12345"));

            var result = Rule().Evaluate(new schemeType(), new producerType
            {
                status = statusType.A,
                producerBusiness = new producerBusinessType
                {
                    Item = new partnershipType
                    {
                        partnershipName = "New Producer Name"
                    }
                }
            }, A<Guid>._);

            Assert.False(result.IsValid);
            Assert.Equal(Core.Shared.ErrorLevel.Warning, result.ErrorLevel);
        }

        [Fact]
        public void Amendment_MigratedProducerExistsWithPrn_ReturnsFailAsWarning_AndIncludesProducerNamesInErrorMessage()
        {
            const string existingProducerName = "existing producer name";
            const string newProducerName = "new producer Name";

            var migratedProducer = A.Fake<MigratedProducer>();
            A.CallTo(() => migratedProducer.ProducerName)
                .Returns(existingProducerName);

            A.CallTo(() => migratedProducerQuerySet.GetMigratedProducer(A<string>._))
                .Returns(migratedProducer);

            var result = Rule().Evaluate(new schemeType(), new producerType
            {
                status = statusType.A,
                producerBusiness = new producerBusinessType
                {
                    Item = new partnershipType
                    {
                        partnershipName = newProducerName
                    }
                }
            }, A<Guid>._);

            Assert.False(result.IsValid);
            Assert.Equal(Core.Shared.ErrorLevel.Warning, result.ErrorLevel);
            Assert.Contains(existingProducerName, result.Message);
            Assert.Contains(newProducerName, result.Message);
        }

        private ProducerNameChange Rule()
        {
            return new ProducerNameChange(producerQuerySet, migratedProducerQuerySet);
        }
    }
}
