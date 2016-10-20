namespace EA.Weee.XmlValidation.Tests.Unit.BusinessValidation.MemberRegistration.Rules.Producer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Core.Shared;
    using FakeItEasy;
    using Weee.Domain.DataReturns;
    using Weee.Domain.Obligation;
    using Weee.Domain.Producer;
    using Xml.MemberRegistration;
    using XmlValidation.BusinessValidation;
    using XmlValidation.BusinessValidation.MemberRegistration.QuerySets;
    using XmlValidation.BusinessValidation.MemberRegistration.Rules.Producer;
    using Xunit;
    using ObligationType = Weee.Domain.Obligation.ObligationType;

    public class ProducerObligationTypeChangeTests
    {
        [Fact]
        public async Task Evaluate_Insert_DoesNotCheckForExistingProducerDetails_DoesNotRetrieveProducerEeeData_AndReturnsPass()
        {
            // Arrange
            var builder = new ProducerObligationTypeChangeBuilder();

            var newProducerDetails = new producerType
            {
                status = statusType.I,
                obligationType = obligationTypeType.B2C
            };

            // Act
            var result = await builder.Build().Evaluate(newProducerDetails);

            // Assert
            A.CallTo(() => builder.ProducerQuerySet.GetLatestProducerForComplianceYearAndScheme(A<string>._, A<string>._, A<Guid>._))
                .MustNotHaveHappened();

            A.CallTo(() => builder.SchemeEeeDataQuerySet.GetLatestProducerEeeData(A<string>._))
                .MustNotHaveHappened();

            Assert.True(result.IsValid);
        }

        [Fact]
        public async Task Evaluate_ProducerObligationChangedToBoth_DoesNotCheckForExistingProducerDetails_DoesNotRetrieveProducerEeeData_AndReturnsPass()
        {
            // Arrange
            var builder = new ProducerObligationTypeChangeBuilder();

            var newProducerDetails = new producerType
            {
                status = statusType.A,
                obligationType = obligationTypeType.Both
            };

            // Act
            var result = await builder.Build().Evaluate(newProducerDetails);

            // Assert
            A.CallTo(() => builder.ProducerQuerySet.GetLatestProducerForComplianceYearAndScheme(A<string>._, A<string>._, A<Guid>._))
                .MustNotHaveHappened();

            A.CallTo(() => builder.SchemeEeeDataQuerySet.GetLatestProducerEeeData(A<string>._))
                .MustNotHaveHappened();

            Assert.True(result.IsValid);
        }

        [Fact]
        public async Task Evaluate_NotExistingProducerDetails_DoesNotRetrieveProducerEeeData_AndReturnsPass()
        {
            // Arrange
            var builder = new ProducerObligationTypeChangeBuilder();

            A.CallTo(() => builder.ProducerQuerySet.GetLatestProducerForComplianceYearAndScheme(A<string>._, A<string>._, A<Guid>._))
                .Returns(null);

            var newProducerDetails = new producerType
            {
                status = statusType.A,
                obligationType = obligationTypeType.B2C
            };

            // Act
            var result = await builder.Build().Evaluate(newProducerDetails);

            // Assert
            A.CallTo(() => builder.ProducerQuerySet.GetLatestProducerForComplianceYearAndScheme(A<string>._, A<string>._, A<Guid>._))
                .MustHaveHappened();

            A.CallTo(() => builder.SchemeEeeDataQuerySet.GetLatestProducerEeeData(A<string>._))
                .MustNotHaveHappened();

            Assert.True(result.IsValid);
        }

        [Fact]
        public async Task Evaluate_ProducerObligationTypeUnchanged_DoesNotRetrieveProducerEeeData_AndReturnsPass()
        {
            // Arrange
            var builder = new ProducerObligationTypeChangeBuilder();

            var existingProducerDetails = A.Fake<ProducerSubmission>();
            A.CallTo(() => existingProducerDetails.ObligationType)
                .Returns(ObligationType.B2C);

            A.CallTo(() => builder.ProducerQuerySet.GetLatestProducerForComplianceYearAndScheme(A<string>._, A<string>._, A<Guid>._))
                .Returns(existingProducerDetails);

            var newProducerDetails = new producerType
            {
                status = statusType.A,
                obligationType = obligationTypeType.B2C
            };

            // Act
            var result = await builder.Build().Evaluate(newProducerDetails);

            // Assert
            A.CallTo(() => builder.ProducerQuerySet.GetLatestProducerForComplianceYearAndScheme(A<string>._, A<string>._, A<Guid>._))
                .MustHaveHappened();

            A.CallTo(() => builder.SchemeEeeDataQuerySet.GetLatestProducerEeeData(A<string>._))
                .MustNotHaveHappened();

            Assert.True(result.IsValid);
        }

        [Fact]
        public async Task Evaluate_NoExistingEeeDataForProducer_ReturnsPass()
        {
            // Arrange
            var builder = new ProducerObligationTypeChangeBuilder();

            var existingProducerDetails = A.Fake<ProducerSubmission>();
            A.CallTo(() => existingProducerDetails.ObligationType)
                .Returns(ObligationType.B2C);

            A.CallTo(() => builder.ProducerQuerySet.GetLatestProducerForComplianceYearAndScheme(A<string>._, A<string>._, A<Guid>._))
                .Returns(existingProducerDetails);

            A.CallTo(() => builder.SchemeEeeDataQuerySet.GetLatestProducerEeeData(A<string>._))
                .Returns((List<EeeOutputAmount>)null);

            var newProducerDetails = new producerType
            {
                status = statusType.A,
                obligationType = obligationTypeType.B2B
            };

            // Act
            var result = await builder.Build().Evaluate(newProducerDetails);

            // Assert
            A.CallTo(() => builder.ProducerQuerySet.GetLatestProducerForComplianceYearAndScheme(A<string>._, A<string>._, A<Guid>._))
                .MustHaveHappened();

            A.CallTo(() => builder.SchemeEeeDataQuerySet.GetLatestProducerEeeData(A<string>._))
                .MustHaveHappened();

            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData(ObligationType.B2B, ObligationType.B2B, ObligationType.B2C)]
        [InlineData(ObligationType.B2B, ObligationType.Both, ObligationType.B2C)]
        [InlineData(ObligationType.B2C, ObligationType.B2C, ObligationType.B2B)]
        [InlineData(ObligationType.B2C, ObligationType.Both, ObligationType.B2B)]
        public async Task Evaluate_ProducerObligationTypeChange_WithConflictDueToExistingEeeData_ReturnsError
            (ObligationType existingProducerEeeDataObligationType, ObligationType existingProducerObligationType, ObligationType newProducerObligationType)
        {
            // Arrange
            var builder = new ProducerObligationTypeChangeBuilder();

            var existingProducerDetails = A.Fake<ProducerSubmission>();
            A.CallTo(() => existingProducerDetails.ObligationType)
                .Returns(existingProducerObligationType);

            A.CallTo(() => builder.ProducerQuerySet.GetLatestProducerForComplianceYearAndScheme(A<string>._, A<string>._, A<Guid>._))
                .Returns(existingProducerDetails);

            var eeeOutputAmount = A.Fake<EeeOutputAmount>();
            A.CallTo(() => eeeOutputAmount.ObligationType)
                .Returns(existingProducerEeeDataObligationType);

            A.CallTo(() => builder.SchemeEeeDataQuerySet.GetLatestProducerEeeData(A<string>._))
                .Returns(new List<EeeOutputAmount> { eeeOutputAmount });

            var newProducerDetails = new producerType
            {
                status = statusType.A,
                obligationType = newProducerObligationType.ToDeserializedXmlObligationType()
            };

            // Act
            var result = await builder.Build().Evaluate(newProducerDetails);

            // Assert
            A.CallTo(() => builder.ProducerQuerySet.GetLatestProducerForComplianceYearAndScheme(A<string>._, A<string>._, A<Guid>._))
                .MustHaveHappened();

            A.CallTo(() => builder.SchemeEeeDataQuerySet.GetLatestProducerEeeData(A<string>._))
                .MustHaveHappened();

            Assert.False(result.IsValid);
            Assert.Equal(ErrorLevel.Error, result.ErrorLevel);
        }

        [Theory]
        [InlineData(ObligationType.B2B, ObligationType.Both, ObligationType.B2B)]
        [InlineData(ObligationType.B2C, ObligationType.Both, ObligationType.B2C)]
        public async Task Evaluate_ProducerObligationTypeChange_NoConflictWithExistingEeeData_ReturnsPass
            (ObligationType existingProducerEeeDataObligationType, ObligationType existingProducerObligationType, ObligationType newProducerObligationType)
        {
            // Arrange
            var builder = new ProducerObligationTypeChangeBuilder();

            var existingProducerDetails = A.Fake<ProducerSubmission>();
            A.CallTo(() => existingProducerDetails.ObligationType)
                .Returns(existingProducerObligationType);

            A.CallTo(() => builder.ProducerQuerySet.GetLatestProducerForComplianceYearAndScheme(A<string>._, A<string>._, A<Guid>._))
                .Returns(existingProducerDetails);

            var eeeOutputAmount = A.Fake<EeeOutputAmount>();
            A.CallTo(() => eeeOutputAmount.ObligationType)
                .Returns(existingProducerEeeDataObligationType);

            A.CallTo(() => builder.SchemeEeeDataQuerySet.GetLatestProducerEeeData(A<string>._))
                .Returns(new List<EeeOutputAmount> { eeeOutputAmount });

            var newProducerDetails = new producerType
            {
                status = statusType.A,
                obligationType = newProducerObligationType.ToDeserializedXmlObligationType()
            };

            // Act
            var result = await builder.Build().Evaluate(newProducerDetails);

            // Assert
            A.CallTo(() => builder.ProducerQuerySet.GetLatestProducerForComplianceYearAndScheme(A<string>._, A<string>._, A<Guid>._))
                .MustHaveHappened();

            A.CallTo(() => builder.SchemeEeeDataQuerySet.GetLatestProducerEeeData(A<string>._))
                .MustHaveHappened();

            Assert.True(result.IsValid);
        }

        [Fact]
        public async Task Evaluate_ProducerObligationTypeChange_WithConflictDueToExistingEeeData_ReturnsDetailsInErrorMessage()
        {
            // Arrange
            var builder = new ProducerObligationTypeChangeBuilder();

            var existingProducerDetails = A.Fake<ProducerSubmission>();
            A.CallTo(() => existingProducerDetails.ObligationType)
                .Returns(ObligationType.B2B);

            A.CallTo(() => builder.ProducerQuerySet.GetLatestProducerForComplianceYearAndScheme(A<string>._, A<string>._, A<Guid>._))
                .Returns(existingProducerDetails);

            var eeeOutputAmount = A.Fake<EeeOutputAmount>();
            A.CallTo(() => eeeOutputAmount.ObligationType)
                .Returns(ObligationType.B2B);

            A.CallTo(() => builder.SchemeEeeDataQuerySet.GetLatestProducerEeeData(A<string>._))
                .Returns(new List<EeeOutputAmount> { eeeOutputAmount });

            var newProducerDetails = new producerType
            {
                status = statusType.A,
                obligationType = obligationTypeType.B2C,
                tradingName = "TestProducer",
                registrationNo = "WEE/MM0001AA"
            };

            // Act
            var result = await builder.Build().Evaluate(newProducerDetails);

            // Assert
            A.CallTo(() => builder.ProducerQuerySet.GetLatestProducerForComplianceYearAndScheme(A<string>._, A<string>._, A<Guid>._))
                .MustHaveHappened();

            A.CallTo(() => builder.SchemeEeeDataQuerySet.GetLatestProducerEeeData(A<string>._))
                .MustHaveHappened();

            Assert.False(result.IsValid);
            Assert.Equal(ErrorLevel.Error, result.ErrorLevel);
            Assert.Contains("TestProducer", result.Message);
            Assert.Contains("WEE/MM0001AA", result.Message);
            Assert.Contains("from B2B", result.Message);
            Assert.Contains("to B2C", result.Message);
        }

        [Theory]
        [InlineData(ObligationType.B2B, ObligationType.B2B, ObligationType.B2C)]
        [InlineData(ObligationType.B2B, ObligationType.Both, ObligationType.B2C)]
        [InlineData(ObligationType.B2C, ObligationType.B2C, ObligationType.B2B)]
        [InlineData(ObligationType.B2C, ObligationType.Both, ObligationType.B2B)]
        public async Task Evaluate_ProducerObligationTypeChange_WithConflictDueToExistingEeeData_ReturnsExistingUniqueEeeDataObligationInErrorMessage
            (ObligationType existingProducerEeeDataObligationType, ObligationType existingProducerObligationType, ObligationType newProducerObligationType)
        {
            // Arrange
            var builder = new ProducerObligationTypeChangeBuilder();

            var existingProducerDetails = A.Fake<ProducerSubmission>();
            A.CallTo(() => existingProducerDetails.ObligationType)
                .Returns(existingProducerObligationType);

            A.CallTo(() => builder.ProducerQuerySet.GetLatestProducerForComplianceYearAndScheme(A<string>._, A<string>._, A<Guid>._))
                .Returns(existingProducerDetails);

            var eeeOutputAmount = A.Fake<EeeOutputAmount>();
            A.CallTo(() => eeeOutputAmount.ObligationType)
                .Returns(existingProducerEeeDataObligationType);

            A.CallTo(() => builder.SchemeEeeDataQuerySet.GetLatestProducerEeeData(A<string>._))
                .Returns(new List<EeeOutputAmount> { eeeOutputAmount });

            var newProducerDetails = new producerType
            {
                status = statusType.A,
                obligationType = newProducerObligationType.ToDeserializedXmlObligationType()
            };

            // Act
            var result = await builder.Build().Evaluate(newProducerDetails);

            // Assert
            A.CallTo(() => builder.ProducerQuerySet.GetLatestProducerForComplianceYearAndScheme(A<string>._, A<string>._, A<Guid>._))
                .MustHaveHappened();

            A.CallTo(() => builder.SchemeEeeDataQuerySet.GetLatestProducerEeeData(A<string>._))
                .MustHaveHappened();

            Assert.False(result.IsValid);
            Assert.Equal(ErrorLevel.Error, result.ErrorLevel);
            Assert.Contains(string.Format("because {0} EEE data has already been submitted", existingProducerEeeDataObligationType), result.Message);
        }

        [Fact]
        public async Task Evaluate_ProducerObligationTypeChange_WithConflictDueToExistingEeeData_ReturnsExistingEeeDataObligationsInErrorMessage()
        {
            // Arrange
            var builder = new ProducerObligationTypeChangeBuilder();

            var existingProducerDetails = A.Fake<ProducerSubmission>();
            A.CallTo(() => existingProducerDetails.ObligationType)
                .Returns(ObligationType.B2B);

            A.CallTo(() => builder.ProducerQuerySet.GetLatestProducerForComplianceYearAndScheme(A<string>._, A<string>._, A<Guid>._))
                .Returns(existingProducerDetails);

            var eeeOutputAmount1 = A.Fake<EeeOutputAmount>();
            A.CallTo(() => eeeOutputAmount1.ObligationType)
                .Returns(ObligationType.B2B);

            var eeeOutputAmount2 = A.Fake<EeeOutputAmount>();
            A.CallTo(() => eeeOutputAmount2.ObligationType)
                .Returns(ObligationType.B2C);

            A.CallTo(() => builder.SchemeEeeDataQuerySet.GetLatestProducerEeeData(A<string>._))
                .Returns(new List<EeeOutputAmount> { eeeOutputAmount1, eeeOutputAmount2 });

            var newProducerDetails = new producerType
            {
                status = statusType.A,
                obligationType = obligationTypeType.B2C
            };

            // Act
            var result = await builder.Build().Evaluate(newProducerDetails);

            // Assert
            A.CallTo(() => builder.ProducerQuerySet.GetLatestProducerForComplianceYearAndScheme(A<string>._, A<string>._, A<Guid>._))
                .MustHaveHappened();

            A.CallTo(() => builder.SchemeEeeDataQuerySet.GetLatestProducerEeeData(A<string>._))
                .MustHaveHappened();

            Assert.False(result.IsValid);
            Assert.Equal(ErrorLevel.Error, result.ErrorLevel);
            Assert.Contains("because both B2B and B2C EEE data have already been submitted", result.Message);
        }

        private class ProducerObligationTypeChangeBuilder
        {
            public IProducerQuerySet ProducerQuerySet { get; private set; }

            public ISchemeEeeDataQuerySet SchemeEeeDataQuerySet { get; private set; }

            public Guid OrganisationId { get; set; }

            public string ComplianceYear { get; set; }

            public ProducerObligationTypeChangeBuilder()
            {
                ProducerQuerySet = A.Fake<IProducerQuerySet>();
                SchemeEeeDataQuerySet = A.Fake<ISchemeEeeDataQuerySet>();
                OrganisationId = Guid.NewGuid();
                ComplianceYear = "2016";
            }

            public ProducerObligationTypeChange Build()
            {
                return new ProducerObligationTypeChange(OrganisationId, ComplianceYear,
                    (x, y) => SchemeEeeDataQuerySet, ProducerQuerySet);
            }
        }
    }
}
