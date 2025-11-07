namespace EA.Weee.XmlValidation.Tests.Unit.BusinessValidation.MemberRegistration.Rules.Producer
{
    using Domain;
    using FakeItEasy;
    using System;
    using Weee.Domain.Obligation;
    using Xml.MemberRegistration;
    using XmlValidation.BusinessValidation.MemberRegistration.QuerySets;
    using XmlValidation.BusinessValidation.MemberRegistration.Rules.Producer;
    using Xunit;
    using schemeType = Xml.MemberRegistration.schemeType;

    public class ProducerSellingTechniqueChangeTests
    {
        private readonly IProducerQuerySet producerQuerySet;

        public ProducerSellingTechniqueChangeTests()
        {
            producerQuerySet = A.Fake<IProducerQuerySet>();

            // By default, nulls returned from queries
            A.CallTo(() => producerQuerySet.GetLatestProducerForComplianceYearAndScheme(A<string>._, A<string>._, A<Guid>._)).Returns(null);
        }

        [Fact]
        public void Insert_ProducerExistsWithSellingTechniqueType_ReturnsPass()
        {
            A.CallTo(() => producerQuerySet.GetLatestProducerForComplianceYearAndScheme(A<string>._, A<string>._, A<Guid>._))
                                           .Returns(FakeProducer.Create(ObligationType.Both, "ABC12345"));

            var result = Rule().Evaluate(new schemeType(), new producerType
            {
                status = statusType.I,
                sellingTechnique = sellingTechniqueType.OnlineMarketplace,
                producerBusiness = new producerBusinessType
                {
                    Item = new partnershipType
                    {
                        partnershipName = "New Producer Name"
                    }
                }
            }, A.Dummy<Guid>());

            Assert.True(result.IsValid);
        }

        [Fact]
        public void Amendment_ProducerExistsWithMatchingSellingTechniqueType_ReturnsFailAsWarning()
        {
            A.CallTo(() => producerQuerySet.GetLatestProducerForComplianceYearAndScheme(A<string>._, A<string>._, A<Guid>._))
                                           .Returns(FakeProducer.Create(ObligationType.Both, "ABC12345"));

            var result = Rule().Evaluate(new schemeType(), new producerType
            {
                status = statusType.A,
                sellingTechnique = sellingTechniqueType.DirectSellingtoEndUser,
                producerBusiness = new producerBusinessType
                {
                    Item = new partnershipType
                    {
                        partnershipName = "New Producer Name"
                    }
                }
            }, A.Dummy<Guid>());

            Assert.False(result.IsValid);
            Assert.Equal(Core.Shared.ErrorLevel.Warning, result.ErrorLevel);
        }

        private ProducerSellingTechniqueChange Rule()
        {
            return new ProducerSellingTechniqueChange(producerQuerySet);
        }
    }
}
