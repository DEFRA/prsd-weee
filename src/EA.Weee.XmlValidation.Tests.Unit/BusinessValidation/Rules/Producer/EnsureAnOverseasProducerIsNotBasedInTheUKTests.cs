namespace EA.Weee.XmlValidation.Tests.Unit.BusinessValidation.Rules.Producer
{
    using EA.Weee.Core.Shared;
    using EA.Weee.XmlValidation.BusinessValidation;
    using EA.Weee.XmlValidation.BusinessValidation.Rules.Producer;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xml.MemberRegistration;
    using Xunit;

    /// <summary>
    /// This class provdes unit tests for the XML validation rule that ensures an overseas
    /// producer is not based in the UK.
    /// </summary>
    public class EnsureAnOverseasProducerIsNotBasedInTheUKTests
    {
        /// <summary>
        /// This test ensures that the rule will pass for a producer with no authorised representative.
        /// </summary>
        [Fact]
        public void Evaluate_ProducerHasNoAuthorisedRepresentative_Passes()
        {
            // Arrange
            EnsureAnOverseasProducerIsNotBasedInTheUK rule = new EnsureAnOverseasProducerIsNotBasedInTheUK();

            producerType producer = new producerType();
            producer.authorisedRepresentative = null;

            // Act
            RuleResult result = rule.Evaluate(producer);

            // Assert
            Assert.Equal(true, result.IsValid);
        }

        /// <summary>
        /// This test ensures that the rule will pass for a producer whose authorised representative
        /// has no overseas producer.
        /// </summary>
        [Fact]
        public void Evaluate_ProducerHasNoOverseasProducer_Passes()
        {
            // Arrange
            EnsureAnOverseasProducerIsNotBasedInTheUK rule = new EnsureAnOverseasProducerIsNotBasedInTheUK();

            authorisedRepresentativeType authorisedRepresentative = new authorisedRepresentativeType();
            authorisedRepresentative.overseasProducer = null;

            producerType producer = new producerType();
            producer.authorisedRepresentative = authorisedRepresentative;

            // Act
            RuleResult result = rule.Evaluate(producer);

            // Assert
            Assert.Equal(true, result.IsValid);
        }

        /// <summary>
        /// This test ensures that the rule will pass for a producer whose authorised representative's
        /// overseas producer has no overseas contact.
        /// </summary>
        [Fact]
        public void Evaluate_ProducerHasNoOverseasContact_Passes()
        {
            // Arrange
            EnsureAnOverseasProducerIsNotBasedInTheUK rule = new EnsureAnOverseasProducerIsNotBasedInTheUK();

            overseasProducerType overseasProducer = new overseasProducerType();
            overseasProducer.overseasContact = null;

            authorisedRepresentativeType authorisedRepresentative = new authorisedRepresentativeType();
            authorisedRepresentative.overseasProducer = overseasProducer;

            producerType producer = new producerType();
            producer.authorisedRepresentative = authorisedRepresentative;

            // Act
            RuleResult result = rule.Evaluate(producer);

            // Assert
            Assert.Equal(true, result.IsValid);
        }

        /// <summary>
        /// This test ensures that the rule will pass for a producer whose authorised representative's
        /// overseas producer has a contact not based in the UK.
        /// </summary>
        [Fact]
        public void Evaluate_ProducerHasOverseasContactInFrance_Passes()
        {
            // Arrange
            EnsureAnOverseasProducerIsNotBasedInTheUK rule = new EnsureAnOverseasProducerIsNotBasedInTheUK();

            addressType address = new addressType();
            address.country = countryType.FRANCE;

            contactDetailsType overseasContact = new contactDetailsType();
            overseasContact.address = address;

            overseasProducerType overseasProducer = new overseasProducerType();
            overseasProducer.overseasContact = overseasContact;

            authorisedRepresentativeType authorisedRepresentative = new authorisedRepresentativeType();
            authorisedRepresentative.overseasProducer = overseasProducer;

            producerType producer = new producerType();
            producer.authorisedRepresentative = authorisedRepresentative;

            // Act
            RuleResult result = rule.Evaluate(producer);

            // Assert
            Assert.Equal(true, result.IsValid);
        }

        /// <summary>
        /// This test ensures that the rule will fail with an "error" for a producer whose authorised
        /// representative's overseas producer has a contact based in the UK.
        /// </summary>
        [Fact]
        public void Evaluate_ProducerHasOverseaseContactInEngland_FailsWithError()
        {
            // Arrange
            EnsureAnOverseasProducerIsNotBasedInTheUK rule = new EnsureAnOverseasProducerIsNotBasedInTheUK();

            addressType address = new addressType();
            address.country = countryType.UKENGLAND;

            contactDetailsType overseasContact = new contactDetailsType();
            overseasContact.address = address;

            overseasProducerType overseasProducer = new overseasProducerType();
            overseasProducer.overseasContact = overseasContact;

            authorisedRepresentativeType authorisedRepresentative = new authorisedRepresentativeType();
            authorisedRepresentative.overseasProducer = overseasProducer;

            producerType producer = new producerType();
            producer.authorisedRepresentative = authorisedRepresentative;

            // Act
            RuleResult result = rule.Evaluate(producer);

            // Assert
            Assert.Equal(false, result.IsValid);
            Assert.Equal(ErrorLevel.Error, result.ErrorLevel);
        }
    }
}
