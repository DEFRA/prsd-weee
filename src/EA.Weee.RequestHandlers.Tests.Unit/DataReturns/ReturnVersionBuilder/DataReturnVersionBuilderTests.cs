namespace EA.Weee.RequestHandlers.Tests.Unit.DataReturns.ReturnVersionBuilder
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

    public class DataReturnVersionBuilderTests
    {
        [Fact]
        public void InvokeBuild_WithoutAddingAnyReturns_ThrowsInvalidOperationException()
        {
            var builder = new DataReturnVersionBuilderHelper().Create();

            Assert.Throws<InvalidOperationException>(() => builder.Build());
        }

        [Fact]
        public async Task Build_ReturnsDataReturnVersionWhenNoErrors()
        {
            var builder = new DataReturnVersionBuilderHelper().CreateWithErrorData(new List<ErrorData>());
            await builder.AddAatfDeliveredAmount("Approval Number", "Facility name", A<WeeeCategory>._, ObligationType.B2C, A<decimal>._);

            var result = builder.Build();

            Assert.NotNull(result.DataReturnVersion);
        }

        [Fact]
        public async Task Build_ReturnsDataReturnVersionAndWarnings_WhenNoErrorsButWithWarnings()
        {
            var warnings = new List<ErrorData> { new ErrorData("A warning", ErrorLevel.Warning) };

            var builder = new DataReturnVersionBuilderHelper().CreateWithErrorData(warnings);
            await builder.AddAatfDeliveredAmount("Approval Number", "Facility name", A<WeeeCategory>._, ObligationType.B2C, A<decimal>._);

            var result = builder.Build();

            Assert.NotNull(result.DataReturnVersion);
            Assert.Equal(warnings, result.ErrorData);
        }

        [Fact]
        public async Task Build_ReturnsNullDataReturnVersionWhenContainsErrors_AndReturnsErrors()
        {
            var errors = new List<ErrorData> { new ErrorData("An Error", ErrorLevel.Error) };
            var builder = new DataReturnVersionBuilderHelper().CreateWithErrorData(errors);
            await builder.AddAatfDeliveredAmount("Approval Number", "Facility name", A<WeeeCategory>._, ObligationType.B2C, A<decimal>._);

            var result = builder.Build();

            Assert.Null(result.DataReturnVersion);
            Assert.Equal(errors, result.ErrorData);
        }

        [Fact]
        public async Task Build_DataReturnVersionBuilder_NoExistingDataReturn_CreatesNewDataReturn()
        {
            var helper = new DataReturnVersionBuilderHelper();

            A.CallTo(() => helper.DataAccess.FetchDataReturnOrDefault())
                .Returns((DataReturn)null);

            var builder = helper.Create();
            await builder.AddAatfDeliveredAmount("Approval Number", "Facility name", A<WeeeCategory>._, ObligationType.B2C, A<decimal>._);

            var result = builder.Build();

            Assert.NotNull(result.DataReturnVersion.DataReturn);
        }

        [Fact]
        public async Task Build_DataReturnVersionBuilder_ExistingDataReturn_ReturnDataReturnVersionWithExistingDataReturn()
        {
            var dataReturn = new DataReturn(A.Dummy<Domain.Scheme.Scheme>(), A.Dummy<Quarter>());

            var helper = new DataReturnVersionBuilderHelper();

            A.CallTo(() => helper.DataAccess.FetchDataReturnOrDefault())
                .Returns(dataReturn);

            var builder = helper.Create();
            await builder.AddAatfDeliveredAmount("Approval Number", "Facility name", A<WeeeCategory>._, ObligationType.B2C, A<decimal>._);

            var result = builder.Build();

            Assert.Equal(dataReturn, result.DataReturnVersion.DataReturn);
        }

        [Fact]
        public async Task AddAatfDeliveredAmount_CreatesAatfDeliveredAmountDomainObject()
        {
            var builder = new DataReturnVersionBuilderHelper().Create();
            await builder.AddAatfDeliveredAmount("Approval Number", "Facility name", A<WeeeCategory>._, ObligationType.B2C, A<decimal>._);

            var result = builder.Build();

            Assert.Equal(1, result.DataReturnVersion.WeeeDeliveredReturnVersion.WeeeDeliveredAmounts.Count);
            Assert.Collection(result.DataReturnVersion.WeeeDeliveredReturnVersion.WeeeDeliveredAmounts,
                r => Assert.Equal("Approval Number", r.AatfDeliveryLocation.ApprovalNumber));
        }

        [Fact]
        public async Task AddAeDeliveredAmount_CreatesAeDeliveredAmountDomainObject()
        {
            var builder = new DataReturnVersionBuilderHelper().Create();
            await builder.AddAeDeliveredAmount("Approval Number", "Operator name", A<WeeeCategory>._, ObligationType.B2C, A<decimal>._);

            var result = builder.Build();

            Assert.Equal(1, result.DataReturnVersion.WeeeDeliveredReturnVersion.WeeeDeliveredAmounts.Count);
            Assert.Collection(result.DataReturnVersion.WeeeDeliveredReturnVersion.WeeeDeliveredAmounts,
                r => Assert.Equal("Approval Number", r.AeDeliveryLocation.ApprovalNumber));
        }

        [Fact]
        public async Task AddWeeeCollectedAmount_CreatesWeeeCollectedAmountDomainObject()
        {
            var type = WeeeCollectedAmountSourceType.Distributor;

            var builder = new DataReturnVersionBuilderHelper().Create();
            await builder.AddWeeeCollectedAmount(type, A<WeeeCategory>._, ObligationType.B2C, A<decimal>._);

            var result = builder.Build();

            Assert.Equal(1, result.DataReturnVersion.WeeeCollectedReturnVersion.WeeeCollectedAmounts.Count);

            Assert.Collection(result.DataReturnVersion.WeeeCollectedReturnVersion.WeeeCollectedAmounts,
                r => Assert.Equal(type, r.SourceType));
        }

        [Fact]
        public async Task AddEeeOutputAmount_CreatesEeeOutputAmountDomainObject_IfNoErrors()
        {
            var helper = new DataReturnVersionBuilderHelper();

            A.CallTo(() => helper.EeeValidator.Validate(A<string>._, A<string>._, A<WeeeCategory>._, A<ObligationType>._, A<decimal>._))
                .Returns(new List<ErrorData>());

            A.CallTo(() => helper.DataAccess.GetRegisteredProducer(A<string>._))
                .Returns(new RegisteredProducer("Registration Number", 2016, A.Dummy<Domain.Scheme.Scheme>()));

            var builder = helper.Create();
            await builder.AddEeeOutputAmount("Registration Number", A<string>._, A<WeeeCategory>._, ObligationType.B2C, A<decimal>._);

            var result = builder.Build();

            Assert.Equal(1, result.DataReturnVersion.EeeOutputReturnVersion.EeeOutputAmounts.Count);
            Assert.Collection(result.DataReturnVersion.EeeOutputReturnVersion.EeeOutputAmounts,
                r => Assert.Equal("Registration Number", r.RegisteredProducer.ProducerRegistrationNumber));
        }

        [Fact]
        public async Task AddEeeOutputAmount_CreatesEeeOutputAmountDomainObject_IfWarningsOnly()
        {
            var helper = new DataReturnVersionBuilderHelper();

            A.CallTo(() => helper.EeeValidator.Validate(A<string>._, A<string>._, A<WeeeCategory>._, A<ObligationType>._, A<decimal>._))
                .Returns(new List<ErrorData> { new ErrorData("A warning", ErrorLevel.Warning) });

            A.CallTo(() => helper.DataAccess.GetRegisteredProducer(A<string>._))
                .Returns(new RegisteredProducer("Registration Number", 2016, A.Dummy<Domain.Scheme.Scheme>()));

            var builder = helper.Create();
            await builder.AddEeeOutputAmount("Registration Number", A<string>._, A<WeeeCategory>._, ObligationType.B2C, A<decimal>._);

            var result = builder.Build();

            Assert.Equal(1, result.DataReturnVersion.EeeOutputReturnVersion.EeeOutputAmounts.Count);
            Assert.Collection(result.DataReturnVersion.EeeOutputReturnVersion.EeeOutputAmounts,
                r => Assert.Equal("Registration Number", r.RegisteredProducer.ProducerRegistrationNumber));
        }

        [Fact]
        public async Task AddEeeOutputAmount_DoesNotCreateEeeOutputAmountDomainObject_IfValidationError()
        {
            var helper = new DataReturnVersionBuilderHelper();

            A.CallTo(() => helper.EeeValidator.Validate(A<string>._, A<string>._, A<WeeeCategory>._, A<ObligationType>._, A<decimal>._))
                .Returns(new List<ErrorData> { new ErrorData("An Error", ErrorLevel.Error) });

            var builder = helper.Create();
            await builder.AddEeeOutputAmount("Registration Number", A<string>._, A<WeeeCategory>._, ObligationType.B2C, A<decimal>._);

            builder.Build();

            A.CallTo(() => helper.DataAccess.GetRegisteredProducer(A<string>._))
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task AddEeeOutputAmount_WithIfValidationErrorsandWarnings_CapturesErrorsAndWarnings()
        {
            var errorsAndWarnings = new List<ErrorData> { new ErrorData("An Error", ErrorLevel.Error), new ErrorData("A warning", ErrorLevel.Warning) };

            var helper = new DataReturnVersionBuilderHelper();

            A.CallTo(() => helper.EeeValidator.Validate(A<string>._, A<string>._, A<WeeeCategory>._, A<ObligationType>._, A<decimal>._))
                .Returns(errorsAndWarnings);

            var builder = helper.Create();
            await builder.AddEeeOutputAmount("Registration Number", A<string>._, A<WeeeCategory>._, ObligationType.B2C, A<decimal>._);

            var result = builder.Build();

            Assert.Equal(2, result.ErrorData.Count);
            Assert.Contains(result.ErrorData, r => r.ErrorLevel == ErrorLevel.Warning);
            Assert.Contains(result.ErrorData, r => r.ErrorLevel == ErrorLevel.Error);
        }

        private class DataReturnVersionBuilderHelper
        {
            public readonly Domain.Scheme.Scheme Scheme;

            public readonly Quarter Quarter;

            public readonly IDataReturnVersionBuilderDataAccess DataAccess;

            private readonly Func<Domain.Scheme.Scheme, Quarter, IDataReturnVersionBuilderDataAccess> dataAccessDelegate;

            public readonly IEeeValidator EeeValidator;

            private readonly Func<Domain.Scheme.Scheme, Quarter,
                Func<Domain.Scheme.Scheme, Quarter, IDataReturnVersionBuilderDataAccess>, IEeeValidator> eeeValidatorDelegate;

            public DataReturnVersionBuilderHelper()
            {
                Scheme = A.Dummy<Domain.Scheme.Scheme>();
                Quarter = A.Dummy<Quarter>();
                EeeValidator = A.Fake<IEeeValidator>();
                DataAccess = A.Fake<IDataReturnVersionBuilderDataAccess>();

                dataAccessDelegate = (x, y) => DataAccess;
                eeeValidatorDelegate = (s, q, z) => EeeValidator;
            }

            public DataReturnVersionBuilder Create()
            {
                return new DataReturnVersionBuilder(Scheme, Quarter, eeeValidatorDelegate, dataAccessDelegate);
            }

            public DataReturnVersionBuilder CreateWithErrorData(List<ErrorData> errorData)
            {
                return new DataReturnVersionBuilderExtension(Scheme, Quarter, eeeValidatorDelegate, dataAccessDelegate, errorData);
            }
        }

        private class DataReturnVersionBuilderExtension : DataReturnVersionBuilder
        {
            public DataReturnVersionBuilderExtension(Domain.Scheme.Scheme scheme, Quarter quarter,
            Func<Domain.Scheme.Scheme, Quarter, Func<Domain.Scheme.Scheme, Quarter, IDataReturnVersionBuilderDataAccess>, IEeeValidator> eeeValidatorDelegate,
            Func<Domain.Scheme.Scheme, Quarter, IDataReturnVersionBuilderDataAccess> dataAccessDelegate, List<ErrorData> errorData)
                : base(scheme, quarter, eeeValidatorDelegate, dataAccessDelegate)
            {
                ErrorData = errorData;
            }
        }
    }
}
