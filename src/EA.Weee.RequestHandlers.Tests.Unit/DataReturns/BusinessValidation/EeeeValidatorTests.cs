namespace EA.Weee.RequestHandlers.Tests.Unit.DataReturns.BusinessValidation
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Shared;
    using Domain.DataReturns;
    using Domain.Lookup;
    using Domain.Producer;
    using FakeItEasy;
    using RequestHandlers.DataReturns.BusinessValidation;
    using RequestHandlers.DataReturns.ReturnVersionBuilder;
    using Xunit;
    using ObligationType = Domain.ObligationType;
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
            var prn = "Test PRN";
            RegisteredProducer producer = new RegisteredProducer(prn, 2016, builder.Scheme);
            A.CallTo(() => builder.DataAccess.GetRegisteredProducer(prn))
                .Returns(producer);

            //Act
            var errors = await builder.InvokeValidate(prn);

            //Assert
            Assert.Empty(errors);
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

            //Act
            var errors = await builder.InvokeValidate(prn);

            //Assert
            Assert.Equal(1, errors.Count);
            ErrorData error = errors[0];
            Assert.Equal(ErrorLevel.Error, error.ErrorLevel);
            Assert.Contains(prn, error.Description);
            Assert.Contains("2016", error.Description);
        }

        [Fact]
        public async Task Validate_WithIncorrectProducerName_ReturnsError()
        {
            // Arrange
            var builder = new EeeValidatorBuilder();
            builder.Year = 2016;

            var prn = "Non-existent PRN";

            A.CallTo(() => builder.DataAccess.GetRegisteredProducer(A<string>._))
                .Returns((RegisteredProducer)null);
        }

        [Fact]
        public async Task Validate_WithProducerNotRegisteredWithScheme_DoesNotPerformProducerNameValidation()
        {
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

            public Task<List<ErrorData>> InvokeValidate(string producerRegistrationNumber = null, string producerName = null)
            {
                string prn = producerRegistrationNumber ?? A.Dummy<string>();
                string name = producerName ?? A.Dummy<string>();

                return Build().Validate(prn, name, A.Dummy<WeeeCategory>(), A.Dummy<ObligationType>(), A.Dummy<decimal>());
            }
        }
    }
}
