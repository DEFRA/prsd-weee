namespace EA.Weee.RequestHandlers.Tests.Unit.DataReturns.BusinessValidation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Shared;
    using Domain.DataReturns;
    using Domain.Lookup;
    using Domain.Producer;
    using FakeItEasy;
    using RequestHandlers.DataReturns.BusinessValidation;
    using RequestHandlers.DataReturns.ReturnVersionBuilder;
    using Xunit;
    using ObligationType = Domain.Obligation.ObligationType;
    using Quarter = Domain.DataReturns.Quarter;
    using Scheme = Domain.Scheme.Scheme;

    public class EeeeValidatorTests
    {
        [Fact]
        public async Task Validate_WithValidData_ReturnsEmptyErrorDataList()
        {
            // Arrange
            var builder = new EeeValidatorBuilder();
            builder.Year = 2016;

            // Setup producer that exists in scheme for compliance year
            var prn = "TestPRN";
            var producerName = "TestProducerName";
            var obligationType = ObligationType.B2C;

            var producerSubmission = A.Fake<ProducerSubmission>();
            A.CallTo(() => producerSubmission.OrganisationName)
                .Returns(producerName);
            A.CallTo(() => producerSubmission.ObligationType)
                .Returns(obligationType);

            var producer = A.Fake<RegisteredProducer>(x => x.WithArgumentsForConstructor(() => new RegisteredProducer(prn, 2016, builder.Scheme)));
            A.CallTo(() => producer.CurrentSubmission)
                .Returns(producerSubmission);

            A.CallTo(() => builder.DataAccess.GetRegisteredProducer(prn))
                .Returns(producer);

            // Act
            var errors = await builder.InvokeValidate(prn, producerName, obligationType);

            // Assert
            Assert.Empty(errors);
        }

        [Fact]
        public async Task Validate_WithValidPRN_ButIncorrectProducerDetails_ReturnsExpectedErrors()
        {
            // Arrange
            var builder = new EeeValidatorBuilder();
            builder.Year = 2016;

            // Setup producer that exists in scheme for compliance year
            var prn = "Test PRN";
            var producerName = "Test Producer Name";
            var obligationType = ObligationType.B2C;

            var producerSubmission = A.Fake<ProducerSubmission>();
            A.CallTo(() => producerSubmission.OrganisationName)
                .Returns(producerName);

            var producer = A.Fake<RegisteredProducer>(x => x.WithArgumentsForConstructor(() => new RegisteredProducer(prn, 2016, builder.Scheme)));
            A.CallTo(() => producer.CurrentSubmission)
                .Returns(producerSubmission);
            A.CallTo(() => producerSubmission.ObligationType)
                .Returns(obligationType);

            A.CallTo(() => builder.DataAccess.GetRegisteredProducer(prn))
                .Returns(producer);

            // Act
            var errors = await builder.InvokeValidate(prn, "Incorrect Name", ObligationType.B2B);

            // Assert
            Assert.Equal(2, errors.Count);
        }

        [Fact]
        public async Task Validate_WithProducerNotRegisteredWithScheme_ReturnsError()
        {
            // Arrange
            var builder = new EeeValidatorBuilder();
            builder.Year = 2016;

            var prn = "Non-existent PRN";

            A.CallTo(() => builder.DataAccess.GetRegisteredProducer(A<string>._))
                .Returns((RegisteredProducer)null);

            // Act
            var errors = await builder.InvokeValidate(prn);

            // Assert
            Assert.Equal(1, errors.Count);
            ErrorData error = errors[0];
            Assert.Equal(ErrorLevel.Error, error.ErrorLevel);
            Assert.Contains(prn, error.Description);
            Assert.Contains("2016", error.Description);
        }

        [Fact]
        public void ValidateProducerName_WithIncorrectProducerName_ReturnsError()
        {
            // Arrange
            var builder = new EeeValidatorBuilder();
            builder.Year = 2016;

            var producerSubmission = A.Fake<ProducerSubmission>();
            A.CallTo(() => producerSubmission.OrganisationName)
                .Returns("ProducerName123");

            var registeredProducer = A.Fake<RegisteredProducer>();
            A.CallTo(() => registeredProducer.CurrentSubmission)
                .Returns(producerSubmission);

            // Act
            var error = builder.Build().ValidateProducerName(registeredProducer, "PRN1234", "IncorrectProducerName");

            // Assert
            Assert.NotNull(error);
            Assert.Equal(ErrorLevel.Error, error.ErrorLevel);
            Assert.Contains("does not match the registered producer name of", error.Description);
            Assert.Contains("ProducerName123", error.Description);
            Assert.Contains("IncorrectProducerName", error.Description);
            Assert.Contains("PRN1234", error.Description);
            Assert.Contains("2016", error.Description);
        }

        [Fact]
        public void ValidateProducerName_WithProducerNamesDifferentCase_DoesNotReturnError()
        {
            // Arrange
            var builder = new EeeValidatorBuilder();
            builder.Year = 2016;

            var producerSubmission = A.Fake<ProducerSubmission>();
            A.CallTo(() => producerSubmission.OrganisationName)
                .Returns("producer name aaa");

            var registeredProducer = A.Fake<RegisteredProducer>();
            A.CallTo(() => registeredProducer.CurrentSubmission)
                .Returns(producerSubmission);

            // Act
            var error = builder.Build().ValidateProducerName(registeredProducer, A.Dummy<string>(), "Producer NamE AAA");

            // Assert
            Assert.Null(error);
        }

        [Theory]
        [InlineData(ObligationType.B2B, ObligationType.B2C)]
        [InlineData(ObligationType.B2C, ObligationType.B2B)]
        public void ValidateProducerObligationType_WithIncorrectObligationType_ReturnsError(ObligationType registeredObligationType, ObligationType eeeObligationType)
        {
            // Arrange
            var builder = new EeeValidatorBuilder();

            var producerSubmission = A.Fake<ProducerSubmission>();
            A.CallTo(() => producerSubmission.ObligationType)
                .Returns(registeredObligationType);

            var registeredProducer = A.Fake<RegisteredProducer>();
            A.CallTo(() => registeredProducer.CurrentSubmission)
                .Returns(producerSubmission);

            // Act
            var error = builder.Build().ValidateProducerObligationType(registeredProducer, "PRN1234", "TestProducerName", eeeObligationType);

            // Assert
            Assert.NotNull(error);
            Assert.Equal(ErrorLevel.Error, error.ErrorLevel);
            Assert.Contains("for one or more categories but is only registered for", error.Description);
            Assert.Contains("PRN1234", error.Description);
            Assert.Contains("TestProducerName", error.Description);
            Assert.Contains(eeeObligationType.ToString(), error.Description);
            Assert.Contains(registeredObligationType.ToString(), error.Description);
        }

        [Theory]
        [InlineData(ObligationType.B2B, ObligationType.B2B)]
        [InlineData(ObligationType.Both, ObligationType.B2B)]
        [InlineData(ObligationType.B2C, ObligationType.B2C)]
        [InlineData(ObligationType.Both, ObligationType.B2C)]
        public void ValidateProducerObligationType_WithCorrectObligationType_DoesNotReturnError(ObligationType registeredObligationType, ObligationType eeeObligationType)
        {
            // Arrange
            var builder = new EeeValidatorBuilder();

            var producerSubmission = A.Fake<ProducerSubmission>();
            A.CallTo(() => producerSubmission.ObligationType)
                .Returns(registeredObligationType);

            var registeredProducer = A.Fake<RegisteredProducer>();
            A.CallTo(() => registeredProducer.CurrentSubmission)
                .Returns(producerSubmission);

            // Act
            var error = builder.Build().ValidateProducerObligationType(registeredProducer, A.Dummy<string>(), A.Dummy<string>(), eeeObligationType);

            // Assert
            Assert.Null(error);
        }

        private class EeeValidatorBuilder
        {
            public int Year;
            public QuarterType Quarter;
            public Scheme Scheme;
            public IDataReturnVersionBuilderDataAccess DataAccess;

            public EeeValidatorBuilder()
            {
                Year = 2016;
                Quarter = QuarterType.Q1;
                Scheme = A.Fake<Scheme>();

                DataAccess = A.Fake<IDataReturnVersionBuilderDataAccess>();
            }

            public EeeValidator Build()
            {
                Func<Scheme, Quarter, IDataReturnVersionBuilderDataAccess> dataAccessDelegate = (x, y) => DataAccess;

                return new EeeValidator(Scheme, new Quarter(Year, Quarter), dataAccessDelegate);
            }

            public Task<List<ErrorData>> InvokeValidate(string producerRegistrationNumber = "Test PRN", string producerName = "Test Producer Name",
                ObligationType obligationType = ObligationType.B2B)
            {
                return Build().Validate(producerRegistrationNumber, producerName, A.Dummy<WeeeCategory>(), obligationType, A.Dummy<decimal>());
            }
        }
    }
}
