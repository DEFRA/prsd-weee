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
            var result = new CompanyRegistrationNumberChange(producerQuerySet).Evaluate(new producerType() { status = statusType.I });

            A.CallTo(() => producerQuerySet.GetLatestCompanyProducers()).MustNotHaveHappened();

            Assert.True(result.IsValid);
        }

        [Fact]
        public void Evaluate_AmendmentNotCompanyProducer_ReturnsPass()
        {
            var result = new CompanyRegistrationNumberChange(producerQuerySet).Evaluate(new producerType() { status = statusType.A });

            A.CallTo(() => producerQuerySet.GetLatestCompanyProducers()).MustNotHaveHappened();

            Assert.True(result.IsValid);
        }
    }
}
