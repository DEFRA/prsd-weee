namespace EA.Weee.RequestHandlers.Tests.Unit.DataReturns.BusinessValidation
{
    using System;
    using System.Threading.Tasks;
    using Core.Shared;
    using Domain.DataReturns;
    using Domain.Producer;
    using FakeItEasy;
    using RequestHandlers.DataReturns;
    using RequestHandlers.DataReturns.BusinessValidation;
    using RequestHandlers.DataReturns.ReturnVersionBuilder;
    using Xunit;
    using Quarter = Domain.DataReturns.Quarter;
    using Scheme = Domain.Scheme.Scheme;
    public class EeeeValidatorTests
    {
        [Fact]
        public async Task Validate_WithValidData_ReturnsEmptyErrorDataList()
        {
            // Arrange
            int complianceYear = 2016;
            Quarter quarter = new Quarter(complianceYear, Domain.DataReturns.QuarterType.Q1);
            Scheme scheme = A.Fake<Scheme>();
            IDataReturnVersionBuilderDataAccess dataAccess = A.Fake<IDataReturnVersionBuilderDataAccess>();
            Func<Domain.Scheme.Scheme, Quarter, IDataReturnVersionBuilderDataAccess> dataAccessDelegate;
            dataAccessDelegate = (s, q) => dataAccess;

            //Setup producer that exists in scheme for compliance year
            String prn = "Test PRN";
            RegisteredProducer producer = new RegisteredProducer(prn, complianceYear, scheme);
            A.CallTo(() => dataAccess.GetRegisteredProducer(prn)).Returns(producer);

            EeeValidator eeeValidator = new EeeValidator(scheme, quarter, dataAccessDelegate);

            //Act
            var errors = await eeeValidator.Validate(prn, "Fake producer", 
                Domain.Lookup.WeeeCategory.LargeHouseholdAppliances, Domain.ObligationType.B2B, new Decimal(5));

            //Assert
            Assert.Empty(errors);
        }

        [Fact]
        public async Task Validate_WithProducerNotRegisteredWithScheme_ReturnsError()
        {
            // Arrange
            int complianceYear = 2016;
            Quarter quarter = new Quarter(complianceYear, Domain.DataReturns.QuarterType.Q1);
            Scheme scheme = A.Fake<Scheme>();
            IDataReturnVersionBuilderDataAccess dataAccess = A.Fake<IDataReturnVersionBuilderDataAccess>();
            Func<Domain.Scheme.Scheme, Quarter, IDataReturnVersionBuilderDataAccess> dataAccessDelegate;
            dataAccessDelegate = (s, q) => dataAccess;

            String prn = "Non-extistant PRN";

            A.CallTo(() => dataAccess.GetRegisteredProducer(A<string>._)).Returns((RegisteredProducer)null);

            EeeValidator eeeValidator = new EeeValidator(scheme, quarter, dataAccessDelegate);

            //Act
            var errors = await eeeValidator.Validate(prn, "Fake producer", 
                Domain.Lookup.WeeeCategory.LargeHouseholdAppliances, Domain.ObligationType.B2B, new Decimal(5));

            //Assert
            Assert.Equal(1, errors.Count);
            ErrorData error = errors[0];
            Assert.Equal(ErrorLevel.Error, error.ErrorLevel);
            Assert.Contains(prn, error.Description);
            Assert.Contains(complianceYear.ToString(), error.Description);
        }
    }
}
